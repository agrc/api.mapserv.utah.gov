import { Footer as UDSFooter } from '@ugrc/utah-design-system/src/components/Footer';
import govOpsLogo from '../../assets/govops-logo.webp';

import { SELF_SERVICE_URL } from 'astro:env/client';

const columnOne = {
  title: 'Main menu',
  links: [
    {
      url: '/',
      title: 'Home',
    },
    {
      url: '/getting-started/',
      title: 'Getting started',
    },
    {
      url: '/privacy/',
      title: 'Privacy policy',
    },
    {
      url: '/docs/',
      title: 'API documentation',
    },
  ],
};
const columnTwo = {
  title: 'Helpful links',
  links: [
    {
      url: `${SELF_SERVICE_URL}/self-service/`,
      title: 'Self service',
    },
    {
      url: `${SELF_SERVICE_URL}/self-service/create-key`,
      title: 'Create a key',
    },
    {
      url: '${SELF_SERVICE_URL}/self-service/keys',
      title: 'Manage keys',
    },
    {
      url: 'https://s.utah.gov/ugrc-newsletter',
      title: 'Sign up for our newsletter',
    },
  ],
};
const columnThree = {
  title: 'Website information',
  links: [
    {
      url: 'https://github.com/agrc/api.mapserv.utah.gov/blob/main/src/explorer/CHANGELOG.md',
      title: 'Change log',
    },
    {
      url: 'https://github.com/agrc/api.mapserv.utah.gov/issues/new',
      title: 'Report an issue',
    },
  ],
};

export default function () {
  return (
    <UDSFooter
      columnOne={columnOne}
      columnTwo={columnTwo}
      columnThree={columnThree}
      renderAddress={() => {
        return (
          <div className="order-last col-span-1 justify-center text-center sm:col-span-3 md:order-first md:col-span-2 md:justify-self-start md:text-start">
            <div>
              <div className="max-w-xs">
                <img
                  src={govOpsLogo.src}
                  width="320"
                  alt="Department of Government Operations"
                  loading="lazy"
                />
              </div>
              <div className="mt-4 text-lg">
                <div className="whitespace-nowrap font-semibold">
                  Division of Technology Services
                </div>
                <div className="whitespace-nowrap font-medium">
                  Utah Geospatial Resource Center
                </div>
              </div>
            </div>
            <div className="mt-4">
              <div>4315 South 2700 West</div>
              <div>Taylorsville, UT 84129</div>
            </div>
          </div>
        );
      }}
    />
  );
}
