import Button from '../design-system/Button';

const OverviewPage = () => (
  <section className="min-h-screen max-w-prose mt-6 justify-center mx-auto space-y-4">
    <h2 className="text-center text-wavy-800 dark:text-slate-200">
      API Key Statistics
    </h2>
    <div className="flex flex-1 justify-center">
      <div className="max-w-fit">
        <div className="rounded-lg dark:bg-slate-900 border dark:border-slate-950 shadow-lg flex flex-1 dark:divide-slate-950 divide-x">
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
    <div className="flex flex-col gap-6 justify-center pt-10">
      <p className="dark:text-slate-200">
        API keys are the invitation to use this API. Every request sent to the
        API requires an API key to be in the query string. They are free to make
        and allow you to track the usage of the key. This is a great metric to
        see how popular an application or feature is. Keys are restricted to a
        website address or a computers IP address. This allows you to control
        who can use the key.
      </p>
      <p className="dark:text-slate-200">
        We have created your first API key for you already. This is a special
        key and can only be used from the{' '}
        <a href="https://gis.utah.gov/data/address-geocoders-locators/#OfficialClient">
          UGRC API Client
        </a>{' '}
        desktop application. If you need to geocode addresses from a website or
        a different application, you will need to create a key specific to that
        use case.
      </p>
    </div>
    <div className="flex gap-6 justify-center pt-12">
      <Button
        size={Button.Sizes.xl}
        color={Button.Colors.primary}
        appearance={Button.Appearances.solid}
      >
        create new key
      </Button>
      <Button
        size={Button.Sizes.xl}
        color={Button.Colors.primary}
        appearance={Button.Appearances.solid}
      >
        manage keys
      </Button>
    </div>
  </section>
);

export default OverviewPage;
