import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { FirebaseAppProvider } from '@ugrc/utah-design-system';
import PropTypes from 'prop-types';
import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { ErrorBoundary } from 'react-error-boundary';
import App from './App.jsx';
import FirebaseContainer from './FirebaseContainer.jsx';
import './index.css';

let firebaseConfig = {
  apiKey: '',
  authDomain: '',
  projectId: '',
  storageBucket: '',
  messagingSenderId: '',
  appId: '',
  measurementId: '',
};

if (import.meta.env.VITE_FIREBASE_CONFIG) {
  firebaseConfig = JSON.parse(import.meta.env.VITE_FIREBASE_CONFIG);
}

const MainErrorFallback = ({ error, resetErrorBoundary }) => {
  return (
    <div className="static flex h-screen w-screen items-center justify-center">
      <div className="flex-col items-center">
        <h1>Something went wrong</h1>
        <pre className="text-red-500">{error.message}</pre>
        <button
          className="w-full rounded-full border p-1"
          onClick={resetErrorBoundary}
        >
          Try again
        </button>
      </div>
    </div>
  );
};
MainErrorFallback.propTypes = {
  error: PropTypes.object,
  resetErrorBoundary: PropTypes.func,
};

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <ErrorBoundary
      FallbackComponent={MainErrorFallback}
      onReset={() => window.location.reload()}
    >
      <QueryClientProvider
        client={
          new QueryClient({
            defaultOptions: {
              queries: {
                refetchOnWindowFocus: false,
              },
            },
          })
        }
      >
        <FirebaseAppProvider config={firebaseConfig}>
          <FirebaseContainer>
            <App />
            <ReactQueryDevtools />
          </FirebaseContainer>
        </FirebaseAppProvider>
      </QueryClientProvider>
    </ErrorBoundary>
  </StrictMode>,
);
