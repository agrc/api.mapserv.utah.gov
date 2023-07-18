import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import '@utahdts/utah-design-system-header/css';
import React from 'react';
import ReactDOM from 'react-dom/client';
import { FirebaseAppProvider } from 'reactfire';
import App from './App.jsx';
import FirebaseContainer from './FirebaseContainer.jsx';

import './index.css';

const config = {
  apiKey: import.meta.env.VITE_FIREBASE_API_KEY,
  appId: import.meta.env.VITE_FIREBASE_APP_ID,
  authDomain: import.meta.env.VITE_FIREBASE_AUTH_DOMAIN,
  messagingSenderId: import.meta.env.VITE_FIREBASE_MESSAGING_SENDER_ID,
  projectId: import.meta.env.VITE_FIREBASE_PROJECT_ID,
  storageBucket: import.meta.env.VITE_FIREBASE_STORAGE_BUCKET,
};

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
      <FirebaseAppProvider firebaseConfig={config}>
        <FirebaseContainer>
          <App />
          <ReactQueryDevtools />
        </FirebaseContainer>
      </FirebaseAppProvider>
    </React.StrictMode>
    ,
  </QueryClientProvider>,
);
