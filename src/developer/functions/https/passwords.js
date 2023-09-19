import { getFirestore } from 'firebase-admin/firestore';
import { debug, warn } from 'firebase-functions/logger';
import crypto from 'node:crypto';
import { safelyInitializeApp } from '../firebase.js';

safelyInitializeApp();
const db = getFirestore();

/**
 * Creates the key and iv for the password hash
 * @param {string} password
 * @param {string} pepper
 * @returns {{key: Buffer, iv: Buffer}}}
 */
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

/**
 * Encrypts the password
 * @param {string} password
 * @param {string} salt
 * @param {string} pepper
 * @returns {string} a base64 encoded string of the encrypted password
 */
export const encryptPassword = (password, salt, pepper) => {
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
 * @param {string} email - the users email address
 * @param {string} password - the users plain text password
 * @param {string} pepper - the pepper used to hash the password
 * @returns {Promise<boolean>} true if the password matched, false otherwise
 */
export const validateClaim = async (email, password, pepper) => {
  email = email.toLowerCase();
  debug('[functions::validateClaim] email:', email);

  const unclaimedAccountSnap = await db
    .collection('clients-unclaimed')
    .doc(email)
    .get();
  // check firestore that email exists if not, return false
  if (!unclaimedAccountSnap.exists) {
    debug('[functions::validateClaim] email not found in firestore');
    return false;
  }
  // if so, get the password hash
  const credentials = unclaimedAccountSnap.data();
  // encrypted the password
  const encrypted = encryptPassword(password, credentials.salt, pepper);
  debug(
    '[functions::validateClaim] comparison:',
    encrypted,
    credentials.password,
  );
  // compare the hashes
  if (encrypted !== credentials.password) {
    // if not, return false with an empty array
    return false;
  }

  return true;
};
