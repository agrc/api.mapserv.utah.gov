import { connectAuthEmulator } from 'firebase/auth';
import { connectFirestoreEmulator } from 'firebase/firestore';

const connectEmulators = (condition, auth, firestore) => {
  if (condition) {
    if (typeof window === 'undefined' || !window['_firebase_auth_emulator']) {
      try {
        connectAuthEmulator(auth, 'http://localhost:9099', {
          disableWarnings: true,
        });
      } catch {
        console.log('auth emulator already connected');
      }
      if (typeof window !== 'undefined') {
        window['_firebase_auth_emulator'] = true;
      }
    }
    if (
      typeof window === 'undefined' ||
      !window['_firebase_firestore_emulator']
    ) {
      try {
        connectFirestoreEmulator(firestore, 'localhost', 8080);
      } catch {
        console.log('firestore emulator already connected');
      }
      if (typeof window !== 'undefined') {
        window['_firebase_firestore_emulator'] = true;
      }
    }
  }
};

export default connectEmulators;
