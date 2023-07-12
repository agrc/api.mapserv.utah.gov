import { getAuth } from 'firebase/auth';
import { getFirestore } from 'firebase/firestore';
import { Suspense } from 'react';
import {
  Outlet,
  Route,
  RouterProvider,
  createBrowserRouter,
  createRoutesFromElements,
} from 'react-router-dom';
import { useFirebaseApp, useSigninCheck, useUser } from 'reactfire';
import connectEmulators from './Emulators';
import Avatar from './components/Avatar';
import ThemeToggle from './components/ThemeToggle';
import Footer from './components/design-system/Footer';
import Header from './components/design-system/Header';

const links = [
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
];

const App = () => {
  const app = useFirebaseApp();
  const firestore = getFirestore(app);
  const auth = getAuth(app);

  const { data } = useSigninCheck();
  const { data: user } = useUser();

  connectEmulators(import.meta.env.DEV, auth, firestore);

  const authenticated = data?.signedIn ?? false;

  const routes = createRoutesFromElements(
    <Route
      element={
        <>
          <Header
            className="bg-slate-100 dark:bg-wavy-900 transition-colors duration-1000"
            links={links}
          >
            <div className="flex flex-1 space-x-2 items-center">
              <img
                src="/logo.svg"
                alt="UGRC API"
                className="hidden sm:block h-16 w-16"
                role="presentation"
              />
              <h1 className="text-slate-700 dark:text-slate-300 text-center">
                UGRC API
              </h1>
              <div className="inline-flex flex-grow justify-end">
                <ThemeToggle />
              </div>
              <div className="inline-flex flex-shrink justify-end px-4">
                <Avatar
                  anonymous={!(data?.signedIn ?? false)}
                  user={user}
                  signOut={() => auth.signOut()}
                />
              </div>
            </div>
          </Header>
          <main>
            <Outlet />
          </main>
          <Footer className="w-full bg-wavy-800 relative" />
        </>
      }
      errorElement={<div>404</div>}
    >
      {authenticated && (
        <>
          <Route path="/" lazy={() => import('./components/page/Overview')} />
          <Route
            path="create"
            lazy={() => import('./components/page/CreateKey')}
          />
          <Route
            path="keys"
            lazy={() => import('./components/page/ManageKeys')}
          />
        </>
      )}
      {!authenticated && (
        <Route path="/" lazy={() => import('./components/page/Landing')} />
      )}
    </Route>,
  );

  const router = createBrowserRouter(routes, {
    future: {
      v7_normalizeFormMethod: true,
    },
  });

  return (
    <Suspense fallback={<div> loading...</div>}>
      <RouterProvider router={router} />
    </Suspense>
  );
};

export default App;
