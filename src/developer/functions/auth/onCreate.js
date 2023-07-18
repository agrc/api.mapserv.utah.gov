import { getFirestore } from 'firebase-admin/firestore';
import { error, info } from 'firebase-functions/logger';

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
  } catch (errorMessage) {
    error('[auth::user::onCreate] firebase error', { errorMessage, user });

    return false;
  }

  return true;
};
