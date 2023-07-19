import { FieldValue, getFirestore } from 'firebase-admin/firestore';
import { debug, info } from 'firebase-functions/logger';
import { generateKey } from '../keys.js';

const db = getFirestore();

export const createKey = async (data) => {
  info('[functions::createKey] creating key for', data);
  const accountId = data.for;

  const key = await getUniqueKey();

  const apiKey = {
    id: key,
    accountId,
    key,
    created: new Date(),
    status: 'active', // active, paused, deleted
    type: data.type,
    mode: data.mode,
    pattern: data.type === 'browser' ? data.pattern : data.ip,
    regularExpression: '',
    machineName: false,
    elevated: false,
    deleted: null,
    disabled: null,
    notes: data.notes,
  };

  await db.runTransaction(async (transaction) => {
    // add key to the collection
    transaction.create(db.collection('keys').doc(apiKey.id), apiKey);

    // add key to the user
    transaction.update(db.collection('clients').doc(accountId), {
      keys: FieldValue.arrayUnion(apiKey),
      keyIds: FieldValue.arrayUnion(apiKey.id),
    });
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
