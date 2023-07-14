import { getAuth } from 'firebase/auth';
import { getFirestore } from 'firebase/firestore';
import { Suspense } from 'react';
import {
  Navigate,
  Route,
  RouterProvider,
  createBrowserRouter,
  createRoutesFromElements,
} from 'react-router-dom';
import { useFirebaseApp, useSigninCheck } from 'reactfire';
import connectEmulators from './Emulators';
import Layout from './components/page/Layout';

const App = () => {
  const app = useFirebaseApp();
  const firestore = getFirestore(app);
  const auth = getAuth(app);

  const { data } = useSigninCheck();

  connectEmulators(import.meta.env.DEV, auth, firestore);

  const routes = createRoutesFromElements(
    createRoutes(data?.signedIn ?? false),
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

const createRoutes = (authenticated) => {
  if (authenticated) {
    return (
      <>
        <Route element={<Layout />}>
          <Route index lazy={() => import('./components/page/Overview')} />
          <Route
            path="create"
            lazy={() => import('./components/page/CreateKey')}
          />
          <Route
            path="keys"
            lazy={() => import('./components/page/ManageKeys')}
          />
        </Route>
        <Route path="*" element={<Navigate to="/" />} />
      </>
    );
  }

  return (
    <>
      <Route element={<Layout />}>
        <Route index lazy={() => import('./components/page/Landing')} />
      </Route>
      <Route path="*" element={<Navigate to="/" />} />
    </>
  );
};

export default App;
