import { getFirestore } from 'firebase-admin/firestore';
import { keyConverter } from './converters.js';

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

  const querySnapshot = await db
    .collection(`clients/${uid}/keys`)
    .where('deleted', '!=', true)
    .withConverter(keyConverter)
    .get();

  const keys = [];
  querySnapshot.forEach((doc) => keys.push(doc.data()));

  return keys;
};
