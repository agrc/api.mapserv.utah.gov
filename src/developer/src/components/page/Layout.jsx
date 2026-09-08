import { useFirebaseAnalytics } from '@ugrc/utah-design-system/contexts/FirebaseAnalyticsProvider';
import { useEffect } from 'react';
import { Outlet, ScrollRestoration, useLocation } from 'react-router';
import Menu from '../design-system/Menu';
import UtahChrome from './UtahChrome';

const Layout = () => {
  return (
    <>
      <div className="flex flex-none flex-col">
        <UtahChrome />
        <Menu />
        <main id="main-content" className="min-h-0 flex-1">
          <ScrollRestoration />
          <Outlet />
        </main>
      </div>
      <div id="utah-footer" />
      <PageViewLogger />
    </>
  );
};

function PageViewLogger() {
  const logEvent = useFirebaseAnalytics();
  const location = useLocation();

  useEffect(() => {
    logEvent('page_view', {
      page_location: location.pathname + location.search,
    });
  }, [location, logEvent]);

  return null;
}

export default Layout;
