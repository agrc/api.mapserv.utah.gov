import { getFirestore } from 'firebase-admin/firestore';
import { debug, info, warn } from 'firebase-functions/logger';
import { safelyInitializeApp } from '../firebase.js';
import { generateKey } from '../keys.js';
import { getKeys } from './keys.js';

safelyInitializeApp();
const db = getFirestore();

/**
 * Creates an api key record, placing it in the keys and user/keys collections
 * @param {{
 *  for: string,
 *  pattern: string,
 *  ip: string,
 *  type: 'browser' | 'server',
 *  mode: 'development' | 'production',
 *  notes: string
 * }} data - The forms data
 * @returns {Promise<string>} The api key id
 */
export const createKey = async (data) => {
  info('[functions::createKey] creating key for', data);
  const accountId = data.for;

  const existingKeys = await getKeys(accountId, false);
  const duplicateKey = getDuplicateKey(existingKeys, data);

  if (duplicateKey) {
    throw new Error(`Duplicate key found: ${duplicateKey}`);
  }

  const key = await getUniqueKey();

  let regularExpression = '';
  if (data.type === 'browser') {
    regularExpression = generateRegexFromPattern(data.pattern);
    if (regularExpression === '') {
      throw new Error('Invalid pattern');
    }
  }

  const apiKey = {
    id: key,
    accountId,
    key,
    created: new Date(),
    flags: {
      deleted: false,
      disabled: false,
      server: data.type === 'server',
      production: data.mode === 'production',
    },
    pattern: data.type === 'browser' ? data.pattern.trim() : data.ip.trim(),
    regularExpression,
    machineName: false,
    elevated: false,
    notes: data.notes,
    claimed: true,
  };

  await db.runTransaction(async (transaction) => {
    // add key to the collection
    transaction.create(db.collection('keys').doc(apiKey.id), apiKey);
  });

  return apiKey.id;
};

/**
 * This generates a unique key by checking if it exists in the database
 * @returns {Promise<string>} The unique key
 */
const getUniqueKey = async () => {
  const key = generateKey();

  debug('[functions::createKey] checking key', key);

  const ref = await db.collection('keys').doc(key).get();

  debug('[functions::createKey] exists', ref.exists);

  if (ref.exists) {
    return getUniqueKey();
  }

  debug('[functions::createKey] using key', key);

  return key;
};

const httpsRegex = /^https?:\/\//i;
const empty = '';
// the characters allowed in a host name; also used for the `*.` subdomain wildcard so it can never cross into the path
const hostCharacters = '[a-z0-9._-]';
const validHost = /^[a-z0-9](?:[a-z0-9._-]*[a-z0-9])?(?::\d+)?$/;
const regexMetacharacters = /[.*+?^${}()|[\]\\/]/g;

/**
 * Escapes every regular expression metacharacter so the value is matched literally
 * @param {string} value - the literal text to embed in a regular expression
 * @returns {string} the escaped text
 */
const escapeRegex = (value) => value.replace(regexMetacharacters, '\\$&');

/**
 * Converts the user friendly url pattern from the self service website into a regular expression.
 * The only special character is `*`: a leading `*.` matches any subdomain and a trailing `*` matches any path.
 * Everything else is matched literally and the host is always anchored, so `example.com` can not match
 * `example.com.evil.net` and `*.example.com` can not match `evil.net/x.example.com`.
 * @param {string} inputPattern - the user friendly basic pattern from the self service website
 * @returns {string} the proper regular expression text, or an empty string when the pattern is not valid
 */
export const generateRegexFromPattern = (inputPattern) => {
  // if no pattern, return empty
  if (!inputPattern) {
    return empty;
  }

  inputPattern = inputPattern.toString().trim().toLowerCase();

  // if pattern is empty, return empty
  if (inputPattern.length < 1) {
    return empty;
  }

  if (inputPattern === '*') {
    return empty;
  }

  // strip http(s)://
  const stripped = inputPattern.replace(httpsRegex, empty);

  // split the host from the path
  const slashIndex = stripped.indexOf('/');
  let host = slashIndex === -1 ? stripped : stripped.substring(0, slashIndex);
  let path = slashIndex === -1 ? empty : stripped.substring(slashIndex);

  // a trailing * means any path: example.com/*, example.com/app/*, example.com*
  if (path.endsWith('*')) {
    path = path.substring(0, path.length - 1);
  } else if (path === empty && host.endsWith('*')) {
    host = host.substring(0, host.length - 1);
  }

  // a leading *. means any subdomain
  let anySubdomain = false;
  if (host.startsWith('*.')) {
    anySubdomain = true;
    host = host.substring(2);
  }

  // wildcards are only supported at the start of the host and at the end of the pattern
  if (host.includes('*') || path.includes('*')) {
    return empty;
  }

  if (!validHost.test(host)) {
    return empty;
  }

  let pattern = '^https?:\\/\\/';

  if (anySubdomain) {
    pattern += `${hostCharacters}+\\.`;
  }

  pattern += escapeRegex(host);

  if (path === empty) {
    // a host without a path allows any port (unless one was given) and must end at the host boundary
    if (!host.includes(':')) {
      pattern += '(?::\\d+)?';
    }

    pattern += '(?:[/?#]|$)';
  } else {
    pattern += escapeRegex(path);
  }

  try {
    new RegExp(pattern);
  } catch (error) {
    warn('[functions::createKey::generateRegexFromPattern] invalid regex from pattern', {
      inputPattern,
      pattern,
      error,
    });

    return empty;
  }

  return pattern;
};

/**
 * Checks to see if the account has a key with the same pattern and mode
 * @param {[]} keys - The keys for the account
 * @param {{
 *  for: string,
 *  pattern: string,
 *  ip: string,
 *  type: 'browser' | 'server',
 *  mode: 'development' | 'production',
 *  notes: string
 * }} data - The forms data
 * @returns {Promise<string?>} The duplicate API key or null
 */
export const getDuplicateKey = (keys, data) => {
  let matchingKey = null;

  if (!keys || keys?.length === 0 || !Array.isArray(keys)) {
    return matchingKey;
  }

  for (const key of keys) {
    // if the modes are different they are not the same key
    if (key?.flags?.production !== (data.mode === 'production')) {
      continue;
    }

    // if the key's pattern or ip matches
    if (key?.pattern === (key?.flags?.server ? data.ip : data.pattern)) {
      matchingKey = key?.key;

      break;
    }
  }

  return matchingKey;
};
