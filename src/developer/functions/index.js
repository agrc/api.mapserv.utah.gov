import { debug } from 'firebase-functions/logger';
import { auth } from 'firebase-functions/v1'; // v2 does not support auth triggers as of july/23
import { https } from 'firebase-functions/v2';
import { safelyInitializeApp } from './firebase.js';

// const { defineSecret } = require('firebase-functions/params');

/**
 * @template CallableRequestT
 * @typedef {import('firebase-functions/v2/https').CallableRequest<CallableRequestT>} CallableRequest
 */

safelyInitializeApp();

// auth
/**
 * This private function creates a user in firestore when someone creates an account with firebase auth.
 * @param {any} async(user - The user details from the OIDC flow from Utahid.
 * @returns {bool} - The true or false result of the create user method.
 */
export const onCreateUser = auth.user().onCreate(async (user) => {
  debug('[auth::user::onCreate] importing createUser');
  const createUser = (await import('./auth/onCreate.js')).createUser;

  const result = await createUser(user);

  debug('[auth::user::onCreate]', result);

  return result;
});

// functions
/**
 * This public https function creates an api key based on form data.
 * @param {CallableRequest<Object>} request - The request object containing the auth and form data.
 * @returns {Promise<string>} The api key string value.
 */
export const createKey = https.onCall(
  { cors: [/ut-dts-agrc-web-api-dev-self-service\.web\.app$/] },
  async (request) => {
    if (request.auth === undefined) {
      debug('[https::createKey] no auth context');

      throw new https.HttpsError(
        https.FunctionsErrorCode.UNAUTHENTICATED,
        'unauthenticated',
      );
    }

    debug('[https::createKey] importing createKey');
    const createKey = (await import('./https/createKey.js')).createKey;

    const result = await createKey(request.data);

    debug('[https::createKey]', result);

    return result.toUpperCase();
  },
);

/**
 * Returns all the keys for a specific user
 * @param {CallableRequest<null>} request - The request object containing the auth and form data.
 * @returns {Promise<{
 *   key: string,
 *   created: string,
 *   createdDate: string,
 *   notes: string
 * }[]>} an array of minimal key objects
 */
export const keys = https.onCall(
  { cors: [/ut-dts-agrc-web-api-dev-self-service\.web\.app$/] },
  async (request) => {
    debug('[https::keys] starting');

    if (request.auth === undefined) {
      debug('[https::keys] no auth context');

      throw new https.HttpsError(
        https.FunctionsErrorCode.UNAUTHENTICATED,
        'unauthenticated',
      );
    }

    debug('[https::keys] importing getKeys');
    const getKeys = (await import('./https/keys.js')).getKeys;

    const result = await getKeys(request.auth.uid);

    debug('[https::getKeys]', result);

    return result;
  },
);
