import { initializeApp } from 'firebase-admin/app';
import { getFirestore } from 'firebase-admin/firestore';
import { debug, info } from 'firebase-functions/logger';
import { generateKey } from '../keys.js';

try {
  initializeApp();
} catch {
  // if already initialized, do nothing
}

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
  // if no pattern, return empty
  if (!pattern) {
    return empty;
  }

  pattern = pattern.toString().trim().toLowerCase();

  // if pattern is empty, return empty
  if (pattern.length < 1) {
    return empty;
  }

  if (pattern === '*') {
    return empty;
  }

  // strip http(s)://
  let stripped = pattern.replace(httpsRegex, empty);

  // escape periods
  let escaped = stripped.replace(/\./g, '\\.');

  // replace *\. with .+\.
  if (escaped.startsWith('*')) {
    if (escaped.startsWith(`*\\.`)) {
      escaped = oneOrMoreOfAny + escaped.substring(1);
    } else {
      escaped = escaped.substring(1);
    }
  }

  // replace /* with /.+
  if (escaped.endsWith('/*')) {
    escaped = escaped.substring(0, escaped.length - 1) + '.*';
  }

  return `^https?://` + escaped;
};
