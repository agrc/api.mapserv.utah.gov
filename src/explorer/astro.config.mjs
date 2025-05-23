import partytown from '@astrojs/partytown';
import starlight from '@astrojs/starlight';
import tailwind from '@astrojs/tailwind';
import { defineConfig, envField } from 'astro/config';
import starlightLinksValidator from 'starlight-links-validator';

import react from '@astrojs/react';

// https://astro.build/config
export default defineConfig({
  integrations: [
    starlightLinksValidator(),
    starlight({
      title: 'UGRC API Documentation',
      logo: {
        src: './src/assets/api.svg',
      },
      social: [
        {
          icon: 'github',
          label: 'GitHub',
          href: 'https://github.com/agrc/api.mapserv.utah.gov',
        },
        { icon: 'x.com', label: 'X', href: 'https://x.com/maputah' },
        {
          icon: 'facebook',
          label: 'Facebook',
          href: 'https://www.facebook.com/utahagrc',
        },
        {
          icon: 'youtube',
          label: 'YouTube',
          href: 'https://youtube.com/@therealugrc',
        },
        {
          icon: 'instagram',
          label: 'Instagram',
          href: 'https://instagram.com/ugrc.gis',
        },
      ],
      sidebar: [
        {
          label: 'Home',
          link: '/',
        },
        {
          label: 'Reference',
          items: [
            {
              label: 'Getting started guide',
              link: '/getting-started/',
            },
            {
              label: 'Documentation overview',
              link: '/docs/',
            },
            {
              label: 'Sample code',
              link: 'https://github.com/agrc/api.mapserv.utah.gov/tree/main/samples',
            },
            {
              label: 'Release notes',
              link: 'https://github.com/agrc/api.mapserv.utah.gov/tags',
            },
            {
              label: 'Report a problem',
              link: 'https://github.com/agrc/api.mapserv.utah.gov/issues/new',
            },
            {
              label: 'Privacy policy',
              link: '/privacy/',
            },
          ],
        },
        {
          label: 'Endpoints (version 1)',
          autogenerate: {
            directory: '/docs/v1/endpoints/',
          },
        },
      ],
      customCss: ['./src/tailwind.css'],
    }),
    tailwind(),
    react(),
    partytown(),
  ],
  env: {
    schema: {
      SELF_SERVICE_URL: envField.string({
        context: 'client',
        access: 'public',
        default: 'http://localhost:5173',
      }),
    },
  },
  vite: {
    resolve: {
      alias: [
        {
          find: 'use-sync-external-store/shim/index.js',
          replacement: 'react',
        },
      ],
    },
  },
});
