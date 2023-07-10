import { useSigninCheck } from 'reactfire';
import LandingPage from './components/page/Landing';
import OverviewPage from './components/page/Overview';

const Routes = () => {
  const { data } = useSigninCheck();

  return data?.signedIn ? <OverviewPage /> : <LandingPage />;
};

export default Routes;
