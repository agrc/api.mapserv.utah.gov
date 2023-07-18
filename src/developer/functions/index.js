import { initializeApp } from 'firebase-admin/app';
import { debug } from 'firebase-functions/logger';
import { auth } from 'firebase-functions/v1'; // v2 does not support this yet
import { https } from 'firebase-functions/v2';

initializeApp();

// auth
export const onCreateUser = auth.user().onCreate(async (user) => {
  debug('[auth::user::onCreate] importing createUser');
  const createUser = (await import('./auth/onCreate.js')).createUser;

  const result = await createUser(user);

  debug('[auth::user::onCreate]', result);

  return result;
});

// firestore
export const createKey = https.onCall({ cors: false }, async (request) => {
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

  return result;
});
