import { getAuth } from 'firebase/auth';
import { getFirestore } from 'firebase/firestore';
import { getFunctions } from 'firebase/functions';
import { Suspense } from 'react';
import {
  Outlet,
  Route,
  RouterProvider,
  createBrowserRouter,
  createRoutesFromElements,
  redirect,
} from 'react-router-dom';
import { useFirebaseApp, useSigninCheck } from 'reactfire';
import connectEmulators from './Emulators';
import Layout from './components/page/Layout';

const App = () => {
  const app = useFirebaseApp();
  const firestore = getFirestore(app);
  const functions = getFunctions(app);
  const auth = getAuth(app);
  const { status, data: signInCheck } = useSigninCheck();
  const { data: user } = useSigninCheck();

  connectEmulators(import.meta.env.DEV, auth, firestore, functions);

  const anonymous = !(signInCheck?.signedIn ?? false);

  let router = {};
  if (status !== 'loading') {
    router = createBrowserRouter(
      createRoutesFromElements(
        <Route element={<Layout />}>
          <Route
            index
            loader={() => promotableRoute(anonymous)}
            lazy={() => import('./components/page/Landing')}
          />
          <Route path="/self-service" Component={Outlet}>
            <Route
              index
              loader={() => protectedRoute(anonymous, user)}
              lazy={() => import('./components/page/SelfService')}
            />
            <Route
              path="create-key"
              loader={() => protectedRoute(anonymous, user)}
              lazy={() => import('./components/page/CreateKey')}
            />
            <Route
              path="keys"
              loader={() => protectedRoute(anonymous, user)}
              lazy={() => import('./components/page/Keys')}
            />
            <Route
              path="keys/:key"
              loader={() => protectedRoute(anonymous, user)}
              lazy={() => import('./components/page/Key')}
            />
            <Route
              path="claim-account"
              loader={() => protectedRoute(anonymous, user)}
              lazy={() => import('./components/page/ClaimAccount')}
            />
          </Route>
          <Route path="*" loader={() => redirect('/')} />
        </Route>,
      ),
      {
        future: {
          v7_normalizeFormMethod: true,
        },
      },
    );
  }

  return (
    <Suspense fallback={<div> loading...</div>}>
      {status === 'loading' ? null : <RouterProvider router={router} />}
    </Suspense>
  );
};

const protectedRoute = (anonymous, user) => {
  if (anonymous) {
    return redirect('/');
  }

  return user;
};

const promotableRoute = (anonymous) => {
  if (!anonymous) {
    return redirect('/self-service');
  }

  return null;
};

export default App;
