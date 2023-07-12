import { RouterButtonLink } from '../design-system/Button';
import { TextLink } from '../Link';

export function Component() {
  return (
    <section className="mx-auto mt-6 min-h-screen max-w-prose justify-center space-y-4">
      <h2 className="text-center text-wavy-800 dark:text-slate-200">
        API Key Statistics
      </h2>
      <div className="flex flex-1 justify-center">
        <div className="max-w-fit">
          <div className="flex flex-1 divide-x rounded-lg border shadow-lg dark:divide-slate-950 dark:border-slate-950 dark:bg-slate-900">
            <div className="p-6 text-center">
              <p className="text-2xl font-semibold text-wavy-800 dark:text-slate-200">
                2,712,908
              </p>
              <p className="text-sm text-wavy-500 dark:text-mustard-500/60">
                requests to date
              </p>
            </div>
            <div className="p-6 text-center">
              <p className="text-2xl font-semibold text-wavy-800 dark:text-slate-200">
                15
              </p>
              <p className="text-sm text-wavy-500 dark:text-mustard-500/60">
                total keys
              </p>
            </div>
          </div>
        </div>
      </div>
      <div className="flex flex-col justify-center gap-6 pt-10">
        <p className="dark:text-slate-200">
          API keys are the invitation to use this API. Every request sent to the
          API requires an API key to be in the query string. They are free to
          make and allow you to track the usage of the key. This is a great
          metric to see how popular an application or feature is. Keys are
          restricted to a website address or a computers IP address. This allows
          you to control who can use the key.
        </p>
        <p className="dark:text-slate-200">
          We have created your first API key for you already. This is a special
          key and can only be used from the{' '}
          <TextLink href="https://gis.utah.gov/data/address-geocoders-locators/#OfficialClient">
            UGRC API Client
          </TextLink>{' '}
          desktop application. If you need to geocode addresses from a website
          or a different application, you will need to create a key specific to
          that use case.
        </p>
      </div>
      <div className="flex justify-center gap-6 pt-12">
        <RouterButtonLink
          to="/create"
          appearance="solid"
          color="primary"
          size="xl"
        >
          create a new key
        </RouterButtonLink>
        <RouterButtonLink
          to="/keys"
          appearance="solid"
          color="primary"
          size="xl"
        >
          manage keys
        </RouterButtonLink>
      </div>
    </section>
  );
}
