import { useFirebaseAuth } from '@ugrc/utah-design-system';
import { Suspense } from 'react';
import {
  Outlet,
  Route,
  RouterProvider,
  createBrowserRouter,
  createRoutesFromElements,
  redirect,
} from 'react-router';
import Layout from './components/page/Layout';

const App = () => {
  const { currentUser } = useFirebaseAuth();

  const anonymous = !currentUser;

  const router = createBrowserRouter(
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
            loader={() => protectedRoute(anonymous, currentUser)}
            lazy={() => import('./components/page/SelfService')}
          />
          <Route
            path="create-key"
            loader={() => protectedRoute(anonymous, currentUser)}
            lazy={() => import('./components/page/CreateKey')}
          />
          <Route
            path="keys"
            loader={() => protectedRoute(anonymous, currentUser)}
            lazy={() => import('./components/page/Keys')}
          />
          <Route
            path="keys/:key"
            loader={() => protectedRoute(anonymous, currentUser)}
            lazy={() => import('./components/page/Key')}
          />
          <Route
            path="claim-account"
            loader={() => protectedRoute(anonymous, currentUser)}
            lazy={() => import('./components/page/ClaimAccount')}
          />
        </Route>
        <Route path="*" loader={() => redirect('/')} />
      </Route>,
    ),
  );

  return (
    <Suspense fallback={<div> loading...</div>}>
      <RouterProvider router={router} />
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
