import {
  Footer,
  Header,
  useFirebaseAnalytics,
  useFirebaseAuth,
} from '@ugrc/utah-design-system';
import { useEffect } from 'react';
import { Outlet, useLocation } from 'react-router';
import Menu from '../design-system/Menu';

const links = [
  {
    action: {
      url: 'https://www.utah.gov/support/disclaimer.html',
      openInNewTab: true,
    },
    key: 'Terms of use',
  },
  {
    action: {
      url: 'https://www.utah.gov/support/privacypolicy.html',
      openInNewTab: true,
    },
    key: 'Privacy policy',
  },
  {
    action: {
      url: 'https://www.utah.gov/support/accessibility.html',
      openInNewTab: true,
    },
    key: 'Accessibility',
  },
  {
    action: {
      url: 'https://www.utah.gov/support/translate.html',
      openInNewTab: true,
    },
    key: 'Translate',
  },
];

const Layout = () => {
  const { currentUser, logout } = useFirebaseAuth();

  return (
    <>
      <Header links={links} currentUser={currentUser} logout={logout}>
        <div className="flex h-full grow items-center gap-3">
          <img
            src="/logo.svg"
            alt="UGRC API"
            className="hidden h-16 w-16 sm:block"
            role="presentation"
          />
          <h2 className="font-heading text-3xl font-black text-zinc-600 sm:text-5xl dark:text-zinc-100">
            UGRC API
          </h2>
        </div>
      </Header>
      <Menu />
      <main className="flex-1">
        <Outlet />
      </main>
      <Footer />
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
