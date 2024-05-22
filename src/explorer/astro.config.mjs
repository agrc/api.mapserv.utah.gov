import starlight from '@astrojs/starlight';
import tailwind from '@astrojs/tailwind';
import { defineConfig } from 'astro/config';
import starlightLinksValidator from 'starlight-links-validator';

import react from '@astrojs/react';

// https://astro.build/config
export default defineConfig({
  integrations: [
    starlightLinksValidator(),
    starlight({
      title: 'UGRC API Documentation',
      // site: "https://ut-dts-agrc-web-api-dev.web.app",
      logo: {
        src: './src/assets/api.svg',
      },
      social: {
        github: 'https://github.com/agrc/api.mapserv.utah.gov',
        'x.com': 'https://x.com/maputah',
        facebook: 'https://www.facebook.com/utahagrc',
        youtube: 'https://youtube.com/@therealugrc',
      },
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
              link: 'https://github.com/agrc/api.mapserv.utah.gov/tree/development/samples',
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
    tailwind({
      applyBaseStyles: true,
    }),
    react(),
  ],
});
