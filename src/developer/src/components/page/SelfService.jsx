import { ExclamationTriangleIcon } from '@heroicons/react/20/solid';
import { useQuery } from '@tanstack/react-query';
import { useFirebaseFunctions } from '@ugrc/utah-design-system';
import { httpsCallable } from 'firebase/functions';
import { Link, useLoaderData } from 'react-router-dom';
import { TextLink } from '../Link';
import Button, { RouterButtonLink } from '../design-system/Button';
import Spinner from '../design-system/Spinner';

const numberFormat = new Intl.NumberFormat('en-US');

export function Component() {
  const { functions } = useFirebaseFunctions();
  const getKeys = httpsCallable(functions, 'keys');
  const loaderData = useLoaderData();

  const { status, data: response } = useQuery({
    queryKey: ['my keys', loaderData.uid],
    queryFn: () => Spinner.minDelay(getKeys()),
    enabled: (loaderData?.uid.length ?? 0 > 0) ? true : false,
    onError: () => 'We had some trouble finding your keys.',
    gcTime: Infinity,
    staleTime: Infinity,
  });

  return (
    <article>
      <section className="relative mb-12 w-full px-6 md:mx-auto">
        <div className="bg-circuit absolute inset-0 h-32 bg-primary-600 shadow-lg"></div>
        <div className="relative space-y-4">
          <h3 className="mb-3 ml-2 pt-3 text-center text-white dark:text-slate-200">
            API key statistics
          </h3>
          <div className="flex flex-1 justify-center">
            <div className="max-w-fit">
              <div className="flex flex-1 divide-x rounded-lg border bg-white shadow-lg dark:divide-slate-950 dark:border-secondary-400/20 dark:bg-slate-900">
                {status === 'pending' && (
                  <div className="flex min-h-[100px] min-w-[266px] items-center justify-center">
                    <Spinner
                      size={Spinner.Sizes.xl}
                      className="text-primary-800"
                      ariaLabel="fetching API statistics"
                    />
                  </div>
                )}
                {status === 'success' && (
                  <>
                    <div className="p-6 text-center">
                      <p className="text-2xl font-semibold text-primary-800 dark:text-slate-200">
                        {numberFormat.format(
                          response.data.reduce(
                            (sum, key) => sum + Number(key.usage ?? 0),
                            0,
                          ),
                        )}
                      </p>
                      <p className="text-sm text-primary-500 dark:text-secondary-500/60">
                        requests to date
                      </p>
                    </div>
                    <div className="p-6 text-center">
                      <div className="text-2xl font-semibold text-primary-800 dark:text-slate-200">
                        {response.data.length}
                      </div>
                      <p className="text-sm text-primary-500 dark:text-secondary-500/60">
                        total keys
                      </p>
                    </div>
                  </>
                )}
                {status === 'error' && (
                  <>
                    <div className="p-6 text-center">
                      <p className="text-2xl font-semibold text-primary-800 dark:text-slate-200">
                        <div className="flex min-h-[32px] items-center justify-center">
                          <span className="sr-only">
                            We were unable to fetch your API key count
                          </span>
                          <ExclamationTriangleIcon className="h-6 w-6 text-secondary-500" />
                        </div>
                      </p>
                      <p className="text-sm text-primary-500 dark:text-secondary-500/60">
                        requests to date
                      </p>
                    </div>
                    <div className="p-6 text-center">
                      <div className="text-2xl font-semibold text-primary-800 dark:text-slate-200">
                        <div className="flex min-h-[32px] items-center justify-center">
                          <span className="sr-only">
                            We were unable to fetch your API key count
                          </span>
                          <ExclamationTriangleIcon className="h-6 w-6 text-secondary-500" />
                        </div>
                      </div>
                      <p className="text-sm text-primary-500 dark:text-secondary-500/60">
                        total keys
                      </p>
                    </div>
                  </>
                )}
              </div>
            </div>
          </div>
        </div>
      </section>
      <section className="mb-6 flex justify-center gap-6">
        <RouterButtonLink
          to="create-key"
          appearance={Button.Appearances.solid}
          color={Button.Colors.primary}
          size={Button.Sizes.xl}
        >
          create a new key
        </RouterButtonLink>
        <RouterButtonLink
          to="keys"
          appearance={Button.Appearances.solid}
          color={Button.Colors.primary}
          size={Button.Sizes.xl}
        >
          manage keys
        </RouterButtonLink>
      </section>
      <section className="mx-auto max-w-prose justify-center space-y-4 p-6">
        <div className="flex flex-col justify-center gap-6">
          <p className="dark:text-slate-200">
            API keys are the invitation to use this API. Every request sent to
            the API requires an API key to be in the query string. They are free
            to generate and allow you to track the usage of the key. This is a
            great metric to see how popular an application or feature is. Keys
            are restricted to a website address or a computer IP address. This
            allows you to control who can use the key.
          </p>
          <p className="dark:text-slate-200">
            We have created your first API key for you already. This is a
            special key and can only be used from the{' '}
            <TextLink href="https://gis.utah.gov/products/sgid/address/api-client/">
              UGRC API Client
            </TextLink>
            . If you need to geocode addresses or search SGID data from a
            website or another application, you will need to create a key
            specific to that use case. You can claim keys that were created
            prior to the Utahid login by{' '}
            <Link
              to="/self-service/claim-account"
              className="font-medium italic text-secondary-900 underline decoration-secondary-500 underline-offset-2 transition-all hover:decoration-2 hover:underline-offset-4 focus:outline-none active:text-secondary-500 active:decoration-secondary-600 active:underline-offset-8 dark:text-secondary-400/70"
            >
              claiming non-Utahid keys
            </Link>
            .
          </p>
          <h4 className="text-center dark:text-slate-200">
            Fair use disclaimer
          </h4>
          <p className="dark:text-slate-200">
            This API is free to use and we ask that you use it responsibly and
            fairly. We reserve the right to block abusive users. Currently, the
            API is not rate limited so please be courteous to other users. If
            you need more bandwidth than we are able to provide, we can set up a
            dedicated instance for you.
          </p>
        </div>
      </section>
    </article>
  );
}
Component.displayName = 'Authenticated';
