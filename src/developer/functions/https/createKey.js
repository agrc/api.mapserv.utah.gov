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

const httpsRegex = /https?:\/\//i;
const oneOrMoreOfAny = '.+';
const empty = '';

/**
 * Description
 * @param {string} inputPattern - the user friendly basic pattern from the self service website
 * @returns {string} the proper regular expression text
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
  let stripped = inputPattern.replace(httpsRegex, empty);

  // escape periods
  let replacements = stripped.replace(/\./g, '\\.');

  // replace *\. with .+\.
  if (replacements.startsWith('*')) {
    if (replacements.startsWith('*\\.')) {
      replacements = oneOrMoreOfAny + replacements.substring(1);
    } else {
      replacements = replacements.substring(1);
    }
  }

  // replace /* with /.+
  if (replacements.endsWith('/*')) {
    replacements = replacements.substring(0, replacements.length - 1) + '.*';
  }

  const pattern = '^https?:\\/\\/' + replacements;

  try {
    new RegExp(pattern);
  } catch {
    warn('[functions::createKey::generateRegexFromPattern] invalid regex from pattern', {
      inputPattern,
      pattern,
    });
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
