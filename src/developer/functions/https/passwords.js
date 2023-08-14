import { initializeApp } from 'firebase-admin/app';
import { getFirestore } from 'firebase-admin/firestore';
import { warn } from 'firebase-functions/logger';
import crypto from 'node:crypto';

initializeApp();
const db = getFirestore();

export const createKeyGen = (password, pepper) => {
  const pepperBytes = Buffer.from(pepper, 'utf8');

  const iterations = 1000;
  const length = 32 + 16;
  const digest = 'sha1';
  const keyPlusIv = crypto.pbkdf2Sync(
    password,
    pepperBytes,
    iterations,
    length,
    digest,
  );

  return {
    key: keyPlusIv.subarray(0, 32),
    iv: keyPlusIv.subarray(32, length),
  };
};

export const hashPassword = (password, salt, pepper) => {
  if (!salt) {
    salt = '';
    warn('[functions::hashPassword] no salt provided to hashPassword function');
  }

  if (!pepper) {
    pepper = '';
    warn(
      '[functions::hashPassword] no pepper provided to hashPassword function',
    );
  }

  const outputEncoding = 'base64';
  const { key, iv } = createKeyGen(password + salt, pepper);
  const passwordAndSalt = Buffer.from(password + salt, 'utf8');

  const cipher = crypto.createCipheriv('aes-256-cbc', key, iv);

  let result = cipher.update(passwordAndSalt, null, outputEncoding);
  result += cipher.final(outputEncoding);

  return result;
};

/**
 * Validates a user account claim request
 * @param {string} userId - the users id from Utah id SSO
 * @param {string} email - the users email address
 * @param {string} password - the users plain text password
 * @param {string} pepper - the pepper used to hash the password
 * @returns {Promise<Object>} - the result object returns the status of the validation and a list of api keys from the legacy account.
 */
export const validateClaim = async (userId, email, password, pepper) => {
  const account = await db.collection('unclaimed-accounts').get(email);
  // check firestore that email exists
  // if not, return false
  if (!account.exists) {
    return false;
  }
  // if so, get the password hash
  const credentials = account.data().password;
  // hash the password
  const hash = hashPassword(password, credentials.salt, pepper);
  // compare the hashes
  if (hash !== credentials.hash) {
    // if not, return false with an empty array
    return { status: false, keys: [] };
  }
  // if they match, return true will all the api keys as an array
  const querySnapshot = await db
    .collection(`keys`)
    .where('accountId', '==', userId)
    .get();

  const keys = [];
  querySnapshot.forEach((doc) => keys.push(doc.data()));

  return { status: true, keys };
};
