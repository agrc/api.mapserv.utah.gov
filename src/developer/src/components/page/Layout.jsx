import { getAuth } from 'firebase/auth';
import { Outlet, useNavigate } from 'react-router-dom';
import { useSigninCheck, useUser } from 'reactfire';
import Avatar from '../Avatar';
import ThemeToggle from '../ThemeToggle';
import Footer from '../design-system/Footer';
import Header from '../design-system/Header';

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

const Layout = () => {
  const navigate = useNavigate();
  const auth = getAuth();
  const { data } = useSigninCheck();
  const { data: user } = useUser();

  return (
    <>
      <Header
        className="bg-slate-100 transition-colors duration-1000 dark:bg-wavy-900"
        links={links}
      >
        <div className="flex flex-1 items-center space-x-2">
          <img
            src="/logo.svg"
            alt="UGRC API"
            className="hidden h-16 w-16 sm:block"
            role="presentation"
          />
          <h1 className="text-center text-slate-700 dark:text-slate-300">
            UGRC API
          </h1>
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
      <main>
        <Outlet />
      </main>
      <Footer className="relative w-full bg-wavy-800" />
    </>
  );
};

export default Layout;
