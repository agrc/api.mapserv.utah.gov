import Header from '@ugrc/header';
import { logEvent } from 'firebase/analytics';
import { getAuth } from 'firebase/auth';
import { useEffect } from 'react';
import { Outlet, useLocation, useNavigate } from 'react-router-dom';
import { useAnalytics, useSigninCheck, useUser } from 'reactfire';
import Avatar from '../Avatar';
import ThemeToggle from '../ThemeToggle';
import Footer from '../design-system/Footer';
import Menu from '../design-system/Menu';

const links = [
  {
    actionUrl: {
      url: 'https://www.utah.gov/support/disclaimer.html',
      openInNewTab: true,
    },
    title: 'Terms of use',
  },
  {
    actionUrl: {
      url: 'https://www.utah.gov/support/privacypolicy.html',
      openInNewTab: true,
    },
    title: 'Privacy policy',
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

const Layout = () => {
  const navigate = useNavigate();
  const auth = getAuth();
  const { data } = useSigninCheck();
  const { data: user } = useUser();

  return (
    <>
      <Header
        className="bg-slate-100 duration-1000 dark:bg-wavy-900"
        links={links}
      >
        <div className="flex flex-1 items-center space-x-2">
          <button
            onClick={() => navigate('/')}
            className="flex items-center space-x-2 rounded-md p-2"
          >
            <img
              src="/logo.svg"
              alt="UGRC API"
              className="hidden h-16 w-16 sm:block"
              role="presentation"
            />
            <h2 className="text-center text-slate-700 dark:text-slate-300">
              UGRC API
            </h2>
          </button>
          <div className="inline-flex flex-grow justify-end">
            <ThemeToggle />
          </div>
          <div className="inline-flex flex-shrink justify-end px-4">
            <Avatar
              anonymous={!(data?.signedIn ?? false)}
              user={user}
              signOut={() => {
                auth.signOut();
                navigate('/');
              }}
            />
          </div>
        </div>
      </Header>
      <Menu />
      <main className="flex-1">
        <Outlet />
      </main>
      <Footer className="w-full bg-wavy-800" />
      <PageViewLogger />
    </>
  );
};

function PageViewLogger() {
  const analytics = useAnalytics();
  const location = useLocation();

  useEffect(() => {
    logEvent(analytics, 'page_view', {
      page_location: location.pathname + location.search,
    });
  }, [location, analytics]);

  return null;
}

export default Layout;
