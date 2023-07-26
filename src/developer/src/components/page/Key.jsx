import {
  BeakerIcon,
  CakeIcon,
  CalendarDaysIcon,
  ChartPieIcon,
  CloudIcon,
  ComputerDesktopIcon,
  CpuChipIcon,
  KeyIcon,
  PauseIcon,
  PlayIcon,
  ShieldExclamationIcon,
} from '@heroicons/react/24/outline';
import { doc } from 'firebase/firestore';
import PropTypes from 'prop-types';
import { useLoaderData, useParams } from 'react-router-dom';
import { useFirestore, useFirestoreDocData } from 'reactfire';
import { timeSince } from '../../../functions/time';
import Button, { RouterButtonLink } from '../design-system/Button';
import Card from '../design-system/Card';
import Spinner from '../design-system/Spinner';

const numberFormat = new Intl.NumberFormat('en-US');
const dateFormatter = new Intl.DateTimeFormat('en-US', {
  year: 'numeric',
  month: 'numeric',
  day: 'numeric',
  hour: 'numeric',
  minute: 'numeric',
  timeZone: 'MST',
});

const iconStyle =
  'dark:fill-wavy-500/50 dark:text-mustard-400/80 drop-shadow-md h-24 fill-mustard-500/20 text-wavy-500/80';

export const Component = () => {
  const { key } = useParams();
  const loaderData = useLoaderData();

  const ref = doc(
    useFirestore(),
    `clients/${loaderData.user.uid}/keys/${key.toLowerCase()}`
  );

  const { status, data } = useFirestoreDocData(ref);

  if (status === 'success' && !data) {
    return (
      <>
        <section className="mx-auto flex max-w-5xl gap-4 p-6 md:col-span-2">
          <KeyIcon className="h-14 fill-mustard-500/20 text-wavy-500/80 drop-shadow-md dark:fill-wavy-500/50 dark:text-mustard-400/80" />
          <div>
            <h2
              id="key-creation"
              className="uppercase text-wavy-600 dark:text-wavy-200"
            >
              {key}
            </h2>
            <p className="text-wavy-400">key does not exist</p>
          </div>
        </section>
        <section className="relative mb-12 w-full px-6 md:mx-auto">
          <div className="bg-circuit absolute inset-0 h-64 bg-wavy-600 shadow-lg"></div>
          <div className="mx-auto w-full">
            <div className="mt-16 flex flex-col items-center gap-2">
              <h3 className="mt-16 text-center text-5xl font-black tracking-tight text-mustard-400 drop-shadow-md">
                This key does not exist!
              </h3>
              <ShieldExclamationIcon className="mb-14 h-24 fill-wavy-500/70 text-mustard-400/90 drop-shadow-md" />
            </div>
          </div>
        </section>
        <section className="mx-auto flex max-w-5xl justify-center gap-4 p-6">
          <RouterButtonLink
            to="/self-service/keys"
            appearance="solid"
            color="primary"
            size="xl"
          >
            Go back to your keys
          </RouterButtonLink>
        </section>
      </>
    );
  }

  return (
    <>
      <section className="mx-auto flex max-w-5xl gap-4 p-6 md:col-span-2">
        <KeyIcon className="h-14 fill-mustard-500/20 text-wavy-500/80 drop-shadow-md dark:fill-wavy-500/50 dark:text-mustard-400/80" />
        <div>
          <h2
            id="key-creation"
            className="uppercase text-wavy-600 dark:text-wavy-200"
          >
            {key}
          </h2>
          <p className="text-wavy-400">
            {status === 'loading' ? 'fetching metadata...' : data?.pattern}
          </p>
        </div>
      </section>
      <section className="relative mb-12 w-full px-6 md:mx-auto">
        <div className="bg-circuit absolute inset-0 h-64 bg-wavy-600 shadow-lg"></div>
        {status === 'loading' ? (
          <div className="flex h-full flex-1 items-center justify-center">
            <Spinner
              size={Spinner.Sizes.custom}
              className="w-16 text-wavy-200"
              ariaLabel="fetching API key metadata"
            />
          </div>
        ) : (
          <>
            <div className="relative z-10 mx-auto max-w-5xl px-6">
              <h3 className="mb-3 ml-2 pt-3 text-center text-white md:col-span-2">
                API Key Metadata
              </h3>
            </div>
            <div className="mx-auto w-full">
              <div className="relative mx-auto flex max-w-7xl flex-1 flex-wrap justify-around gap-2 px-6 sm:gap-8 lg:gap-12">
                <Card className="min-w-[250px]" title="Creation Date">
                  <MetadataItem>
                    <CakeIcon className={iconStyle} />
                    <Banner>
                      {dateFormatter.format(
                        Date.parse(data?.created.toDate().toISOString())
                      )}
                    </Banner>
                  </MetadataItem>
                </Card>
                <Card title="Type">
                  <MetadataItem>
                    {data?.type === 'browser' ? (
                      <ComputerDesktopIcon className={iconStyle} />
                    ) : (
                      <CpuChipIcon className={iconStyle} />
                    )}
                    <Banner>{data?.type}</Banner>
                  </MetadataItem>
                </Card>
                <Card className="min-w-[250px]" title="Status">
                  <MetadataItem>
                    {data?.status === 'active' ? (
                      <PlayIcon className={iconStyle} />
                    ) : (
                      <PauseIcon className={iconStyle} />
                    )}
                    <Banner>{data?.status}</Banner>
                  </MetadataItem>
                </Card>
                <Card className="min-w-[250px]" title="Mode">
                  <MetadataItem>
                    {data?.mode === 'live' ? (
                      <CloudIcon className={iconStyle} />
                    ) : (
                      <BeakerIcon className={iconStyle} />
                    )}
                    <Banner>{data?.mode}</Banner>
                  </MetadataItem>
                </Card>
                <Card className="min-w-[250px]" title="Usage">
                  <MetadataItem>
                    <ChartPieIcon className={iconStyle} />
                    <Banner>
                      {(data?.usage ?? 'none') === 'none'
                        ? 'none'
                        : numberFormat.format(data.usage)}
                    </Banner>
                  </MetadataItem>
                </Card>
                <Card className="min-w-[250px]" title="Last Used">
                  <MetadataItem>
                    <CalendarDaysIcon className={iconStyle} />
                    <Banner>
                      {data?.lastUsed ?? 'never' === 'never'
                        ? 'never'
                        : timeSince(
                            Date.parse(data.lastUsed.toDate().toISOString())
                          )}
                    </Banner>
                  </MetadataItem>
                </Card>
              </div>
            </div>
          </>
        )}
      </section>
      {status === 'success' && data && (
        <section className="mx-auto mb-12 max-w-5xl p-6 md:col-span-2">
          <Card title="Key Notes">
            <div className="flex flex-col items-center gap-4 p-4 text-wavy-800 dark:text-wavy-200">
              {data?.pattern === 'api-client.ugrc.utah.gov'
                ? 'This API key is special and can only be used with the UGRC API Client. It enabled desktop geocoding of CSV files of addresses.'
                : data?.notes}
              <Button>Edit</Button>
            </div>
          </Card>
        </section>
      )}
    </>
  );
};
Component.displayName = 'Key';

const MetadataItem = ({ children }) => (
  <div className="flex w-[250px] flex-col justify-center gap-4">{children}</div>
);
MetadataItem.propTypes = {
  children: PropTypes.node.isRequired,
};

const Banner = ({ children }) => (
  <div className="flex h-12 w-[250px] items-center justify-center border-y border-y-slate-300 bg-slate-200 px-2 dark:border-y-slate-500/50 dark:bg-slate-600 dark:text-wavy-100">
    <p className="overflow-hidden text-ellipsis">{children}</p>
  </div>
);
Banner.displayName = 'Banner';
Banner.propTypes = {
  children: PropTypes.node,
};
