import tailwindcss from '@tailwindcss/vite';
import react from '@vitejs/plugin-react';
import { defineConfig } from 'vite';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react(), tailwindcss()],
  test: {
    env: 'node',
    provider: 'v8',
  },
  optimizeDeps: {
    include: ['firebase/app', 'firebase/auth', 'firebase/analytics', 'firebase/firestore', 'firebase/functions'],
  },
  resolve: {
    // this is only applicable when pnpm-linking the utah-design-package
    // dedupe: ['firebase', 'react'],
    alias: [
      {
        find: 'use-sync-external-store/shim/index.js',
        replacement: 'react',
      },
    ],
  },
});
