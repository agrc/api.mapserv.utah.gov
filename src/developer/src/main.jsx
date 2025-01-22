import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { FirebaseAppProvider } from '@ugrc/utah-design-system';
import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { ErrorBoundary } from 'react-error-boundary';
import App from './App.jsx';
import { MainErrorFallback } from './components/ErrorFallback.jsx';
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

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <ErrorBoundary FallbackComponent={MainErrorFallback} onReset={() => window.location.reload()}>
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
