import { getFirestore } from 'firebase-admin/firestore';
import { debug, info, warn } from 'firebase-functions/logger';
import { safelyInitializeApp } from '../firebase';
import { generateKey } from '../keys.js';

safelyInitializeApp();
const db = getFirestore();

export const createKey = async (data) => {
  info('[functions::createKey] creating key for', data);
  const accountId = data.for;

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
    status: 'active', // active, paused, deleted
    type: data.type,
    mode: data.mode,
    pattern: data.type === 'browser' ? data.pattern : data.ip,
    regularExpression,
    machineName: false,
    elevated: false,
    deleted: false,
    disabled: false,
    notes: data.notes,
    lastUsed: 'never',
    usage: 'none',
    claimed: true,
  };

  await db.runTransaction(async (transaction) => {
    // add key to the collection
    transaction.create(db.collection('keys').doc(apiKey.id), apiKey);

    // add key to the user
    transaction.create(
      db.collection(`clients/${accountId}/keys`).doc(apiKey.id),
      apiKey,
    );
  });

  return apiKey.id;
};

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

export const generateRegexFromPattern = (pattern) => {
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
    if (replacements.startsWith(`*\\.`)) {
      replacements = oneOrMoreOfAny + replacements.substring(1);
    } else {
      replacements = replacements.substring(1);
    }
  }

  // replace /* with /.+
  if (replacements.endsWith('/*')) {
    replacements = replacements.substring(0, replacements.length - 1) + '.*';
  }

  // eslint-disable-next-line no-useless-escape
  const pattern = `^https?:\/\/` + replacements;

  try {
    new RegExp(pattern);
  } catch {
    warn(
      '[functions::createKey::generateRegexFromPattern] invalid regex from pattern',
      {
        inputPattern,
        pattern,
      },
    );
  }

  return pattern;
};
