import { initializeApp } from 'firebase-admin/app';
import { debug } from 'firebase-functions/logger';
import { auth } from 'firebase-functions/v1'; // v2 does not support this yet

initializeApp();

// auth
export const onCreateUser = auth.user().onCreate(async (user) => {
  debug('[auth::user::onCreate] importing createUser');
  const createUser = (await import('./auth/onCreate.js')).createUser;

  const result = await createUser(user);

  debug('[auth::user::onCreate]', result);

  return result;
});

