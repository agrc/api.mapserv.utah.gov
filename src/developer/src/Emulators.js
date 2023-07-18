import { connectAuthEmulator } from 'firebase/auth';
import { connectFirestoreEmulator } from 'firebase/firestore';
import { connectFunctionsEmulator } from 'firebase/functions';

const connectEmulators = (condition, auth, firestore, functions) => {
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
      !window['_firebase_functions_emulator']
    ) {
      try {
        connectFunctionsEmulator(functions, 'localhost', 5001);
      } catch {
        console.log('functions emulator already connected');
      }
      if (typeof window !== 'undefined') {
        window['_firebase_function_emulator'] = true;
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
