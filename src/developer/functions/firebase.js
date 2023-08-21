import { initializeApp } from 'firebase-admin/app';

export const safelyInitializeApp = () => {
  try {
    initializeApp();
  } catch {
    // if already initialized, do nothing
  }
};
