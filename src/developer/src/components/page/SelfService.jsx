import { ExclamationTriangleIcon } from '@heroicons/react/20/solid';
import { useQuery } from '@tanstack/react-query';
import { httpsCallable } from 'firebase/functions';
import { useLoaderData } from 'react-router-dom';
import { useFunctions } from 'reactfire';
import { TextLink } from '../Link';
import { RouterButtonLink } from '../design-system/Button';
import Spinner from '../design-system/Spinner';

export function Component() {
  const functions = useFunctions();
  const getKeys = httpsCallable(functions, 'keys');
  const loaderData = useLoaderData();

  const { status, data: response } = useQuery({
    queryKey: ['my keys', loaderData.user.uid],
    queryFn: () => Spinner.minDelay(getKeys()),
    enabled: loaderData.user?.uid.length ?? 0 > 0 ? true : false,
    onError: () => 'We had some trouble finding your keys.',
    cacheTime: Infinity,
  });

  return (
    <article className="">
      <section className="relative mb-12 w-full px-6 md:mx-auto">
        <div className="bg-circuit absolute inset-0 h-32 bg-wavy-600 shadow-lg"></div>
        <div className="relative space-y-4">
          <h3 className="mb-3 ml-2 pt-3 text-center text-white dark:text-slate-200">
            API Key Statistics
          </h3>
          <div className="flex flex-1 justify-center">
            <div className="max-w-fit">
              <div className="flex flex-1 divide-x rounded-lg border bg-white shadow-lg dark:divide-slate-950 dark:border-mustard-400/20 dark:bg-slate-900">
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
                    {status === 'success' && response.data.length}
                    {status === 'error' && (
                      <div className="flex min-h-[32px] items-center justify-center">
                        <span className="sr-only">
                          We were unable to fetch your API key count
                        </span>
                        <ExclamationTriangleIcon className="h-6 w-6 text-mustard-500" />
                      </div>
                    )}
                    {status === 'loading' && (
                      <div className="flex min-h-[32px] items-center justify-center">
                        <Spinner
                          size={Spinner.Sizes.xl}
                          className="text-wavy-800"
                          ariaLabel="fetching API key count"
                        />
                      </div>
                    )}
                  </p>
                  <p className="text-sm text-wavy-500 dark:text-mustard-500/60">
                    total keys
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>
      <section className="mb-6 flex justify-center gap-6">
        <RouterButtonLink
          to="create-key"
          appearance="solid"
          color="primary"
          size="xl"
        >
          create a new key
        </RouterButtonLink>
        <RouterButtonLink
          to="keys"
          appearance="solid"
          color="primary"
          size="xl"
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
            special key and can only be used from the desktop application. If
            you need to geocode addresses from a website or a different
            application, you will need to create a key specific to that use
            case. You can claim keys that were created prior to the Utahid login
            by{' '}
            <TextLink href="/self-service/claim-account">
              claiming legacy keys
            </TextLink>
            .
          </p>
        </div>
      </section>
    </article>
  );
}
Component.displayName = 'Authenticated';
