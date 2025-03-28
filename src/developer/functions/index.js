import { debug } from 'firebase-functions/logger';
import { defineSecret } from 'firebase-functions/params';
import { runWith } from 'firebase-functions/v1';
import { https, setGlobalOptions } from 'firebase-functions/v2';
import { safelyInitializeApp } from './firebase.js';

safelyInitializeApp();

const serviceAccount = `firestore-function-sa@${process.env.GCLOUD_PROJECT}.iam.gserviceaccount.com`;
const vpcConnector = 'memorystore-connector';
const vpcConnectorEgressSettings = 'PRIVATE_RANGES_ONLY';
const secret = defineSecret('SENDGRID_API_KEY');

setGlobalOptions({
  serviceAccount,
  vpcConnector,
  vpcConnectorEgressSettings,
});

const cors = [
  /ut-dts-agrc-web-api-dev-self-service\.web\.app$/,
  /ut-dts-agrc-web-api-prod-self-service\.web\.app$/,
  /api\.mapserv\.utah\.gov$/,
  /developer\.mapserv\.utah\.gov$/,
];

/**
 * @template CallableRequestT
 * @typedef {import('firebase-functions/v2/https').CallableRequest<CallableRequestT>} CallableRequest
 */

// auth
/**
 * This private function creates a user in firestore when someone creates an account with firebase auth.
 * @param {any} async(user - The user details from the OIDC flow from Utahid.
 * @returns {bool} - The true or false result of the create user method.
 */
export const onCreateUser = runWith({
  serviceAccount,
  vpcConnector,
  vpcConnectorEgressSettings,
  secrets: [secret],
})
  .auth.user()
  .onCreate(async (user) => {
    debug('[auth::user::onCreate] importing createUser');
    const createUser = (await import('./auth/onCreate.js')).createUser;

    const result = await createUser(user);

    debug('[auth::user::onCreate]', result);

    debug('debug mode', process.env.NODE_ENV);
    if (process.env.NODE_ENV !== 'development') {
      const mailingListSignUp = (await import('./mail.js')).mailingListSignUp;

      const mailListResult = await mailingListSignUp(
        { displayName: user.displayName, email: user.email },
        process.env.SENDGRID_API_KEY ?? '',
      );

      debug('[auth::user::mailingListSignUp]', mailListResult);
    }

    return result;
  });

// functions
/**
 * This public https function creates an api key based on form data.
 * @param {CallableRequest<Object>} request - The request object containing the auth and form data.
 * @returns {Promise<string>} The api key string value.
 */
export const createKey = https.onCall({ cors }, async (request) => {
  if (request.auth === undefined) {
    debug('[https::createKey] no auth context');

    throw new https.HttpsError('unauthenticated', 'requires authentication');
  }

  debug('[https::createKey] importing createKey');
  const createKey = (await import('./https/createKey.js')).createKey;

  try {
    const result = await createKey(request.data);

    debug('[https::createKey]', result);

    return result.toUpperCase();
  } catch (error) {
    debug('[https::createKey]', error.message);

    if (error.message.startsWith('Duplicate key found')) {
      throw new https.HttpsError('already-exists', 'duplicate key', error.message.split(':')[1].trim());
    }

    throw new https.HttpsError('invalid-argument', error.message);
  }
});

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
export const keys = https.onCall({ cors }, async (request) => {
  debug('[https::keys] starting');

  if (request.auth === undefined) {
    debug('[https::keys] no auth context');

    throw new https.HttpsError('unauthenticated', 'requires authentication');
  }

  debug('[https::keys] importing getKeys');
  const getKeys = (await import('./https/keys.js')).getKeys;

  const result = await getKeys(request.auth.uid);

  debug('[https::getKeys]', result.length);

  return result;
});

/**
 * This private https function is used to validate a non-Utahid account claim.
 * @param {CallableRequest<{email: string, password: string}>} request - The request object should contain the email address and the plain text password to hash and validate.
 * @returns {Promise<string[]>} The keys transferred from the non-Utahid account.
 */
export const validateClaim = https.onCall(
  {
    cors,
    secrets: ['LEGACY_PEPPER'],
  },
  async (request) => {
    if (request.auth === undefined) {
      debug('[https::validateClaim] no auth context');

      throw new https.HttpsError('unauthenticated', 'requires authentication');
    }

    if (!request.data.email || !request.data.password) {
      debug('[https::validateClaim] missing email or password');

      throw new https.HttpsError('invalid-argument', 'missing arguments');
    }

    debug('[https::validateClaim] importing validatePassword');
    const validateClaim = (await import('./https/passwords.js')).validateClaim;
    const userId = request.auth.uid;

    request.data.email = request.data.email.toLowerCase().trim();

    const result = await validateClaim(request.data.email, request.data.password, process.env.LEGACY_PEPPER ?? '');

    debug('[https::validateClaim]', result);

    if (result) {
      debug('[https::validateClaim] transferring keys');
      const transferKeys = (await import('./https/keys.js')).transferKeys;

      const keys = await transferKeys(request.data.email, userId);

      return { keys, message: 'success' };
    }

    return { keys: [], message: 'invalid account information' };
  },
);
