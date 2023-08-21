import { getFirestore } from 'firebase-admin/firestore';
import { safelyInitializeApp } from '../firebase.js';
import { minimalKeyConversion } from './converters.js';

safelyInitializeApp();
const db = getFirestore();
const empty = [];

/**
 * A function that returns the keys for a given user.
 * @param {string} uid - the id of the user to get keys for
 * @returns {Promise<{
 *   key: string,
 *   created: string,
 *   createdDate: string,
 *   notes: string
 * }[]>} an array of keys run through the minimalKeyConversion converter
 */
export const getKeys = async (uid) => {
  if (!uid) {
    return empty;
  }

  const querySnapshot = await db
    .collection('/keys')
    .where('deleted', '!=', true)
    .where('accountId', '==', uid)
    .withConverter(minimalKeyConversion)
    .get();

  const keys = [];
  querySnapshot.forEach((doc) => keys.push(doc.data()));

  return keys;
};
