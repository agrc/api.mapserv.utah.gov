import { connectAuthEmulator, getAuth } from 'firebase/auth';
import { connectFirestoreEmulator, getFirestore } from 'firebase/firestore';
import { AuthProvider, useFirebaseApp } from 'reactfire';
import Routes from './Routes';
import ThemeToggle from './components/ThemeToggle';
import Footer from './components/design-system/Footer';
import Header from './components/design-system/Header';

function App() {
  const app = useFirebaseApp();
  const auth = getAuth(app);
  const firestore = getFirestore(app);

  if (import.meta.env.DEV) {
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

  return (
    <>
      <Header
        className="bg-slate-100 dark:bg-wavy-900 transition-colors duration-1000"
        links={[
          {
            actionUrl: {
              url: 'https://www.utah.gov/support/disclaimer.html',
              openInNewTab: true,
            },
            title: 'Terms of Use',
          },
          {
            actionUrl: {
              url: 'https://www.utah.gov/support/privacypolicy.html',
              openInNewTab: true,
            },
            title: 'Privacy Policy',
          },
          {
            actionUrl: {
              url: 'https://www.utah.gov/support/accessibility.html',
              openInNewTab: true,
            },
            title: 'Accessibility',
          },
          {
            actionUrl: {
              url: 'https://www.utah.gov/support/translate.html',
              openInNewTab: true,
            },
            title: 'Translate',
          },
        ]}
      >
        <div className="flex flex-1 space-x-2 items-center">
          <img
            src="/logo.svg"
            alt="UGRC API"
            className="hidden sm:block max-h-16 w-auto"
            role="presentation"
          />
          <h1 className="text-slate-700 dark:text-slate-300 text-center">
            UGRC API
          </h1>
          <div className="inline-flex flex-1 justify-end px-4">
            <ThemeToggle />
          </div>
        </div>
      </Header>
      <main>
        <AuthProvider sdk={auth}>
          <Routes />
        </AuthProvider>
      </main>
      <Footer className="w-full bg-wavy-800 fixed lg:relative bottom-0" />
    </>
  );
}

export default App;
