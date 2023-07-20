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
import Button, { RouterButtonLink } from '../design-system/Button';
import Card from '../design-system/Card';
import Spinner from '../design-system/Spinner';

const numberFormat = new Intl.NumberFormat('en-US');
const iconStyle =
  'dark:fill-wavy-500/50 dark:text-mustard-400/80 drop-shadow-md h-24 fill-mustard-500/20 text-wavy-500/80';

export const Component = () => {
  const { key } = useParams();
  const loaderData = useLoaderData();

  const { status, data } = useFirestoreDocData(
    doc(useFirestore(), `clients/${loaderData.user.uid}/keys/${key}`),
  );

  console.log(status, data);

  if (status === 'loading') {
    return (
      <>
        <section className="max-w-5xl mx-auto md:col-span-2 p-6 flex gap-4">
          <KeyIcon className="dark:fill-wavy-500/50 dark:text-mustard-400/80 drop-shadow-md h-14 fill-mustard-500/20 text-wavy-500/80" />
          <div>
            <h2
              id="key-creation"
              className="text-wavy-600 dark:text-wavy-200 uppercase"
            >
              {key}
            </h2>
            <p className="text-wavy-400">fetching metadata</p>
          </div>
        </section>
        <section className="relative w-full md:mx-auto mb-12 px-6">
          <div className="bg-circuit absolute inset-0 h-64 bg-wavy-600 shadow-lg"></div>
          <div className="mx-auto w-full">
            <Spinner ariaLabel="fetching key metadata" />
          </div>
        </section>
      </>
    );
  }

  if (!data) {
    return (
      <>
        <section className="max-w-5xl mx-auto md:col-span-2 p-6 flex gap-4">
          <KeyIcon className="dark:fill-wavy-500/50 dark:text-mustard-400/80 drop-shadow-md h-14 fill-mustard-500/20 text-wavy-500/80" />
          <div>
            <h2
              id="key-creation"
              className="text-wavy-600 dark:text-wavy-200 uppercase"
            >
              {key}
            </h2>
            <p className="text-wavy-400">key does not exist</p>
          </div>
        </section>
        <section className="relative w-full md:mx-auto mb-12 px-6">
          <div className="bg-circuit absolute inset-0 h-64 bg-wavy-600 shadow-lg"></div>
          <div className="mx-auto w-full">
            <div className="flex flex-col gap-2 items-center mt-16">
              <h3 className="text-center text-5xl font-black tracking-tight text-mustard-400 drop-shadow-md">
                This key does not exist!
              </h3>
              <ShieldExclamationIcon className="fill-wavy-500/70 text-mustard-400/90 drop-shadow-md h-24 mb-14" />
              <RouterButtonLink
                to="/self-service/keys"
                appearance="solid"
                color="primary"
                size="xl"
              >
                Go back to your keys
              </RouterButtonLink>
            </div>
          </div>
        </section>
      </>
    );
  }

  return (
    <>
      <section className="max-w-5xl mx-auto md:col-span-2 p-6 flex gap-4">
        <KeyIcon className="dark:fill-wavy-500/50 dark:text-mustard-400/80 drop-shadow-md h-14 fill-mustard-500/20 text-wavy-500/80" />
        <div>
          <h2
            id="key-creation"
            className="text-wavy-600 dark:text-wavy-200 uppercase"
          >
            {key}
          </h2>
          <p className="text-wavy-400">{data.pattern}</p>
        </div>
      </section>
      <section className="relative w-full md:mx-auto mb-12 px-6">
        <div className="bg-circuit absolute inset-0 h-64 bg-wavy-600 shadow-lg"></div>
        <div className="relative z-10 mx-auto max-w-5xl px-6">
          {data && (
            <h3 className="mb-3 ml-2 pt-3 text-center text-white md:col-span-2">
              API Key Metadata
            </h3>
          )}
        </div>
        <div className="mx-auto w-full">
          <div className="relative flex-wrap flex gap-2 sm:gap-8 lg:gap-12 flex-1 justify-around max-w-7xl mx-auto px-6">
            <Card className="min-w-[250px]" title="Creation Date">
              <MetadataItem>
                <CakeIcon className={iconStyle} />
                <Banner>{Date.parse(data.created)}</Banner>
              </MetadataItem>
            </Card>
            <Card title="Type">
              <MetadataItem>
                {data.type === 'browser' ? (
                  <ComputerDesktopIcon className={iconStyle} />
                ) : (
                  <CpuChipIcon className={iconStyle} />
                )}
                <Banner>{data.type}</Banner>
              </MetadataItem>
            </Card>
            <Card className="min-w-[250px]" title="Status">
              <MetadataItem>
                {data.status === 'active' ? (
                  <PlayIcon className={iconStyle} />
                ) : (
                  <PauseIcon className={iconStyle} />
                )}
                <Banner>{data.status}</Banner>
              </MetadataItem>
            </Card>
            <Card className="min-w-[250px]" title="Mode">
              <MetadataItem>
                {data.mode === 'live' ? (
                  <CloudIcon className={iconStyle} />
                ) : (
                  <BeakerIcon className={iconStyle} />
                )}
                <Banner>{data.mode}</Banner>
              </MetadataItem>
            </Card>
            <Card className="min-w-[250px]" title="Usage">
              <MetadataItem>
                <ChartPieIcon className={iconStyle} />
                <Banner>{numberFormat.format(data.usage)}</Banner>
              </MetadataItem>
            </Card>
            <Card className="min-w-[250px]" title="Creation Date">
              <MetadataItem>
                <CalendarDaysIcon className={iconStyle} />
                <Banner>{Date.parse(data.lastUsed)}</Banner>
              </MetadataItem>
            </Card>
          </div>
        </div>
      </section>
      <section className="max-w-5xl mx-auto md:col-span-2 p-6 mb-12">
        <Card title="Key Notes">
          <div className="p-4 text-wavy-800 dark:text-wavy-200 flex flex-col items-center gap-4">
            {data?.notes}
            <Button>Edit</Button>
          </div>
        </Card>
      </section>
    </>
  );
};
Component.displayName = 'Key';

const MetadataItem = ({ children }) => <MetadataItem>{children}</MetadataItem>;
MetadataItem.propTypes = {
  children: PropTypes.node.isRequired,
};

const Banner = ({ children }) => (
  <div className="bg-slate-200 dark:bg-slate-600 dark:border-y-slate-500/50 h-12 border-y border-y-slate-300 items-center flex justify-center">
    {children}
  </div>
);
Banner.displayName = 'Banner';
Banner.propTypes = {
  children: PropTypes.node.isRequired,
};
