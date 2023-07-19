import { getFirestore } from 'firebase-admin/firestore';
import { myKeysConverter } from './converters.js';

const db = getFirestore();
const empty = [];

/**
 * A function that returns the keys for a given user.
 * @param {string} uid
 * @returns {Promise.<Object[]>} keys
 */
export const getKeys = async (uid) => {
  if (!uid) {
    return empty;
  }

  const clientRef = await db
    .collection('clients')
    .doc(uid)
    .withConverter(myKeysConverter)
    .get();

  if (clientRef.exists === false) {
    return empty;
  }

  return clientRef.data();
};
