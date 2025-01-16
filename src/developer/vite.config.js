import react from '@vitejs/plugin-react-swc';
import { defineConfig } from 'vite';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  test: {
    env: 'node',
    provider: 'v8',
  },
  resolve: {
    // this is only applicable when pnpm-linking the utah-design-package
    dedupe: ['firebase', 'react'],
  },
});
