import { getFirestore } from 'firebase-admin/firestore';
import { debug, error } from 'firebase-functions/logger';
import { Redis } from 'ioredis';
import { safelyInitializeApp } from '../firebase.js';
import { standardKeyConversion } from './converters.js';

safelyInitializeApp();
const db = getFirestore();
const empty = [];

let redis = {};
try {
  redis = new Redis(6379, process.env.REDIS_HOST);
} catch (ex) {
  error(ex);
}
/**
 * A function that returns the keys for a given user.
 * @param {string} uid - the id of the user to get keys for
 * @param {boolean} includeAnalytics - whether to include the analytics lookup
 * @returns {Promise<{
 *   key: string,
 *   created: string,
 *   createdDate: string,
 *   notes: string
 * }[]>} an array of keys run through the minimalKeyConversion converter
 */
export const getKeys = async (uid, includeAnalytics = true) => {
  if (!uid) {
    return empty;
  }

  const querySnapshot = await db
    .collection('/keys')
    .where('flags.deleted', '==', false)
    .where('accountId', '==', uid)
    .withConverter(standardKeyConversion)
    .get();

  const keys = [];
  querySnapshot.forEach((doc) => keys.push(doc.data()));

  if (includeAnalytics) {
    const usageKeys = keys.map((key) => `analytics:hit:${key.key.toLowerCase()}`);
    const timeKeys = keys.map((key) => `analytics:time:${key.key.toLowerCase()}`);

    const allKeys = [...usageKeys, ...timeKeys];

    debug('getting usage information for', allKeys);

    if (allKeys.length === 0) {
      return keys;
    }

    try {
      let results = await redis.mget(...allKeys);

      results.forEach((result, index) => {
        if (index < keys.length) {
          keys[index].usage = result ?? 0;
        } else {
          keys[index - keys.length].lastUsed = result ?? -1;
        }
      });
    } catch (ex) {
      error('redis error', ex);
    }
  }

  return keys;
};

/**
 * Transfers keys to a new client
 * @param {string} from - the unclaimed account email to transfer keys from
 * @param {string} to - the new document id to assign the keys to
 * @returns {Promise<claimedKeys: string[]}>} the keys transferred
 */
export const transferKeys = async (from, to) => {
  if (!to || !from) {
    return false;
  }

  const batch = db.batch();

  const keys = await db.collection('/keys').where('accountId', '==', from).get();
  debug('[functions::transferKeys] collected keys:', keys.size);

  const claimedKeys = [];
  if (keys.size === 0) {
    return claimedKeys;
  }

  // move every key to the new client
  keys.forEach(async (key) => {
    claimedKeys.push(key.id);

    batch.update(key.ref, {
      accountId: to,
      claimed: true,
      notes: `📦 transferred from ${from} 📦`,
    });
  });
  debug('[functions::transferKeys] moved keys to new client', to);
  // add email to claimedAccounts on the claiming account
  batch.set(db.collection(`/clients/${to}/claimedAccounts`).doc(from), {
    email: from,
  });
  debug('[functions::transferKeys] added new claimed account to new client', from, `/clients/${to}/claimedAccounts`);
  // remove unclaimed account as it's no longer needed
  batch.delete(db.collection('/clients-unclaimed').doc(from));
  debug('[functions::transferKeys] removed unclaimed account', from);
  // commit the batch
  await batch.commit();

  return claimedKeys;
};
