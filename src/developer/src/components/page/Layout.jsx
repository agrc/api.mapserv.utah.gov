import { Footer, Header, useFirebaseAnalytics, useFirebaseAuth } from '@ugrc/utah-design-system';
import { useEffect } from 'react';
import { Outlet, useLocation } from 'react-router';
import Menu from '../design-system/Menu';
import ThemeToggle from '../ThemeToggle';

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
    <div className="flex min-h-screen flex-col">
      <Header links={links} currentUser={currentUser} logout={logout}>
        <div className="flex h-full grow items-center gap-3">
          <img src="/logo.svg" alt="UGRC API" className="hidden h-16 w-16 sm:block" role="presentation" />
          <h2 className="font-heading text-3xl font-black text-zinc-600 dark:text-zinc-100 sm:text-5xl">UGRC API</h2>
          <span class="mr-6 flex grow justify-end">
            <ThemeToggle />
          </span>
        </div>
      </Header>
      <Menu />
      <main className="flex-1">
        <Outlet />
      </main>
      <Footer
        columnOne={{
          title: 'Main menu',
          links: [
            {
              url: '/',
              title: 'Home',
            },
            {
              url: '/self-service/create-key',
              title: 'Create a key',
            },
            {
              url: '/self-service/keys',
              title: 'Manage keys',
            },
            {
              url: '/self-service/claim-account',
              title: 'Claim keys',
            },
          ],
        }}
        columnTwo={{
          title: 'Helpful links',
          links: [
            {
              url: `${import.meta.env.VITE_API_EXPLORER_URL}/`,
              title: 'UGRC API homepage',
            },
            {
              url: `${import.meta.env.VITE_API_EXPLORER_URL}/docs/`,
              title: 'UGRC API documentation',
            },
          ],
        }}
        columnThree={{
          title: 'Website information',
          links: [
            {
              url: 'https://github.com/agrc/api.mapserv.utah.gov/issues/new',
              title: 'Report an issue',
            },
          ],
        }}
      />
      <PageViewLogger />
    </div>
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
