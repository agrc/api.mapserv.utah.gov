import { getFirestore } from 'firebase-admin/firestore';
import { error, info } from 'firebase-functions/logger';
import { createKey } from '../https/createKey.js';

const db = getFirestore();

export const createUser = async (user) => {
  info('[auth::user::onCreate] adding user to client connection', user);

  const data = {
    email: user.email,
    displayName: user.displayName,
    created: new Date(),
    claimedAccounts: [],
    keys: [],
    keyIds: [],
  };

  try {
    await db.collection('clients').doc(user.uid).set(data);
    await createKey({
      for: user.uid,
      type: 'browser',
      mode: 'production',
      pattern: 'api-client.ugrc.utah.gov',
      notes: '[api-client]',
    });
  } catch (errorMessage) {
    error('[auth::user::onCreate] firebase error', { errorMessage, user });

    return false;
  }

  return true;
};
