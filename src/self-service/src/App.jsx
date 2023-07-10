import { getAuth } from 'firebase/auth';
import { AuthProvider, useFirebaseApp } from 'reactfire';
import Routes from './Routes';
import Footer from './components/design-system/Footer';
import Header from './components/design-system/Header';
import ThemeToggle from './components/page/ThemeToggle';

function App() {
  const app = useFirebaseApp();
  const auth = getAuth(app);

  return (
    <>
      <Header
        className="bg-slate-100 dark:bg-wavy-900 transition-colors duration-1000"
        links={[
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
        ]}
      >
        <div className="flex flex-1 space-x-2 items-center">
          <img
            src="/logo.svg"
            alt="UGRC API"
            className="hidden sm:block max-h-16 w-auto"
            role="presentation"
          />
          <h1 className="text-slate-700 dark:text-slate-300 text-center">
            UGRC API
          </h1>
          <div className="inline-flex flex-1 justify-end px-4">
            <ThemeToggle />
          </div>
        </div>
      </Header>
      <main>
        <AuthProvider sdk={auth}>
          <Routes />
        </AuthProvider>
      </main>
      <Footer className="w-full bg-wavy-800 fixed lg:relative bottom-0" />
    </>
  );
}

export default App;
