import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import React from 'react';
import ReactDOM from 'react-dom/client';
import { FirebaseAppProvider } from 'reactfire';
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

ReactDOM.createRoot(document.getElementById('root')).render(
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
    <React.StrictMode>
      <FirebaseAppProvider firebaseConfig={firebaseConfig}>
        <FirebaseContainer>
          <App />
          <ReactQueryDevtools />
        </FirebaseContainer>
      </FirebaseAppProvider>
    </React.StrictMode>
  </QueryClientProvider>,
);
