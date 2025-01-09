import {
  PauseCircleIcon,
  PlayCircleIcon,
  TrashIcon,
} from '@heroicons/react/20/solid';
import {
  BeakerIcon,
  CakeIcon,
  CalendarDaysIcon,
  ChartPieIcon,
  CloudIcon,
  ComputerDesktopIcon,
  CpuChipIcon,
  ExclamationCircleIcon,
  KeyIcon,
  PauseIcon,
  PlayIcon,
  ShieldExclamationIcon,
} from '@heroicons/react/24/outline';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { doc, updateDoc } from 'firebase/firestore';
import { httpsCallable } from 'firebase/functions';
import PropTypes from 'prop-types';
import { useRef } from 'react';
import { useLoaderData, useNavigate, useParams } from 'react-router-dom';
import { useFirestore, useFunctions } from 'reactfire';
import { timeSince } from '../../../functions/time';
import CopyToClipboard from '../CopyToClipboard';
import EditableText from '../EditableText';
import Button, { RouterButtonLink } from '../design-system/Button';
import Card from '../design-system/Card';
import Spinner from '../design-system/Spinner';

const numberFormat = new Intl.NumberFormat('en-US');

const iconStyle =
  'dark:fill-primary-500/50 dark:text-secondary-400/80 drop-shadow-md h-24 fill-secondary-500/20 text-primary-500/80';

const convertTicks = (ticks) => {
  const ticksToMilliseconds = ticks / 10000;
  const epochMilliseconds = ticksToMilliseconds - 62135596800000;

  return new Date(epochMilliseconds);
};

export const Component = () => {
  const { key } = useParams();
  const keyRef = useRef(doc(useFirestore(), `/keys/${key?.toLowerCase()}`));
  const functions = useFunctions();
  const getKeys = httpsCallable(functions, 'keys');
  const loaderData = useLoaderData();
  const queryClient = useQueryClient();
  const navigate = useNavigate();

  const { status, data } = useQuery({
    queryKey: ['my keys', loaderData.user.uid],
    queryFn: () => Spinner.minDelay(getKeys(), 800),
    select: (response) =>
      response.data.find((data) => data.key === key?.toUpperCase()),
    enabled: (loaderData.user?.uid.length ?? 0 > 0) ? true : false,
    onError: () => 'We had some trouble finding your keys.',
    gcTime: Infinity,
    staleTime: Infinity,
  });

  const prefetchKeys = async () => {
    await queryClient.prefetchQuery({
      queryKey: ['my keys', loaderData.user.uid],
      queryFn: getKeys,
    });
  };

  const cancelAndInvalidate = async () => {
    await queryClient.cancelQueries();
    queryClient.invalidateQueries({
      queryKey: ['my keys'],
      refetchType: 'all',
    });

    prefetchKeys();
  };

  const mutateNotes = async (notes) => {
    await updateDoc(keyRef.current, {
      notes,
    });

    await cancelAndInvalidate();
  };

  const pauseKey = async () => {
    if (window.confirm('Are you sure you want to pause this key?')) {
      await updateDoc(keyRef.current, {
        'flags.disabled': true,
      });

      await cancelAndInvalidate();
    }
  };

  const resumeKey = async () => {
    if (window.confirm('Are you sure you want to resume this key?')) {
      await updateDoc(keyRef.current, {
        'flags.disabled': false,
      });

      await cancelAndInvalidate();
    }
  };

  const deleteKey = async () => {
    if (window.confirm('Are you sure you want to delete this key?')) {
      await updateDoc(keyRef.current, {
        'flags.deleted': true,
      });

      await cancelAndInvalidate();

      navigate('/self-service/keys');
    }
  };

  if (status === 'success' && !data) {
    return (
      <>
        <section className="mx-auto flex max-w-5xl gap-4 p-6 md:col-span-2">
          <KeyIcon className="h-14 fill-secondary-500/20 text-primary-500/80 drop-shadow-md dark:fill-primary-500/50 dark:text-secondary-400/80" />
          <div>
            <h2
              id="key-creation"
              className="uppercase text-primary-600 dark:text-primary-200"
            >
              {key}
            </h2>
            <p className="text-primary-400">key does not exist</p>
          </div>
        </section>
        <section className="relative mb-12 w-full px-6 md:mx-auto">
          <div className="bg-circuit absolute inset-0 h-64 bg-primary-600 shadow-lg"></div>
          <div className="mx-auto w-full">
            <div className="mt-16 flex flex-col items-center gap-2">
              <h3 className="mt-16 text-center text-5xl font-black tracking-tight text-secondary-400 drop-shadow-md">
                This key does not exist!
              </h3>
              <ShieldExclamationIcon className="mb-14 h-24 fill-primary-500/70 text-secondary-400/90 drop-shadow-md" />
            </div>
          </div>
        </section>
        <section className="mx-auto flex max-w-5xl justify-center gap-4 p-6">
          <RouterButtonLink
            to="/self-service/keys"
            appearance={Button.Appearances.solid}
            color={Button.Colors.primary}
            size={Button.Sizes.xl}
          >
            Go back to your keys
          </RouterButtonLink>
        </section>
      </>
    );
  }

  if (status === 'error') {
    return (
      <>
        <section className="relative mb-12 w-full px-6 md:mx-auto">
          <div className="bg-circuit absolute inset-0 h-64 bg-primary-600 shadow-lg"></div>
          <div className="mx-auto w-full">
            <div className="mt-16 flex flex-col items-center gap-2">
              <h3 className="mt-2 text-center text-5xl font-black tracking-tight text-secondary-400 drop-shadow-md">
                We are sorry, but
              </h3>
              <p className="max-w-lg text-center text-xl tracking-wide text-secondary-400 drop-shadow-md">
                we encountered an issue while processing your request. Please
                try again. If the problem persists, please contact our support
                team.
              </p>
              <ExclamationCircleIcon className="mb-14 h-24 fill-primary-500/70 text-secondary-400/90 drop-shadow-md" />
            </div>
          </div>
        </section>
        <section className="mx-auto flex max-w-5xl justify-center gap-4 p-6">
          <RouterButtonLink
            to="/self-service/keys"
            appearance={Button.Appearances.solid}
            color={Button.Colors.primary}
            size={Button.Sizes.xl}
          >
            Go back to your keys
          </RouterButtonLink>
        </section>
      </>
    );
  }

  return (
    <>
      <section className="mx-auto flex max-w-5xl items-center justify-between gap-4 p-6 md:col-span-2">
        <div className="flex gap-4">
          <KeyIcon className="h-14 fill-secondary-500/20 text-primary-500/80 drop-shadow-md dark:fill-primary-500/50 dark:text-secondary-400/80" />
          <div>
            <h2
              id="key-creation"
              className="flex items-center uppercase text-primary-600 dark:text-primary-200"
            >
              {key}
              <CopyToClipboard text={key} className="ml-1 inline" />
            </h2>
            <p className="text-primary-400">
              {status === 'pending' ? 'fetching metadata...' : data?.pattern}
            </p>
          </div>
        </div>
      </section>
      <section className="relative mb-4 w-full px-6 md:mx-auto">
        <div className="bg-circuit absolute inset-0 h-64 bg-primary-600 shadow-lg"></div>
        {status === 'pending' ? (
          <div className="flex h-64 flex-1 items-center justify-center">
            <Spinner
              size={Spinner.Sizes.custom}
              className="w-16 text-primary-200"
              ariaLabel="fetching API key metadata"
            />
          </div>
        ) : (
          <>
            <div className="relative z-10 mx-auto max-w-5xl px-6">
              <h3 className="mb-3 ml-2 pt-3 text-center text-white md:col-span-2">
                API key metadata
              </h3>
            </div>
            <div className="mx-auto w-full">
              <div className="relative mx-auto flex max-w-7xl flex-1 flex-wrap justify-around gap-2 px-6 sm:gap-8 lg:gap-12">
                <Card className="min-w-[250px]" title="Creation Date">
                  <MetadataItem>
                    <CakeIcon className={iconStyle} />
                    <Banner>{data?.createdDate}</Banner>
                  </MetadataItem>
                </Card>
                <Card title="Type">
                  <MetadataItem>
                    {data?.flags.server ? (
                      <CpuChipIcon className={iconStyle} />
                    ) : (
                      <ComputerDesktopIcon className={iconStyle} />
                    )}
                    <Banner>{data?.flags.server ? 'server' : 'browser'}</Banner>
                  </MetadataItem>
                </Card>
                <Card className="min-w-[250px]" title="Status">
                  <MetadataItem>
                    {data?.flags.disabled ? (
                      <PauseIcon className={iconStyle} />
                    ) : (
                      <PlayIcon className={iconStyle} />
                    )}
                    <Banner>
                      {data?.flags.disabled ? 'paused' : 'active'}
                    </Banner>
                  </MetadataItem>
                </Card>
                <Card className="min-w-[250px]" title="Mode">
                  <MetadataItem>
                    {data?.flags.production ? (
                      <CloudIcon className={iconStyle} />
                    ) : (
                      <BeakerIcon className={iconStyle} />
                    )}
                    <Banner>
                      {data?.flags.production ? 'live' : 'development'}
                    </Banner>
                  </MetadataItem>
                </Card>
                <Card className="min-w-[250px]" title="Usage">
                  <MetadataItem>
                    <ChartPieIcon className={iconStyle} />
                    <Banner>
                      {(data?.usage ?? 0) === 0
                        ? 'none'
                        : numberFormat.format(data.usage)}
                    </Banner>
                  </MetadataItem>
                </Card>
                <Card className="min-w-[250px]" title="Last Used">
                  <MetadataItem>
                    <CalendarDaysIcon className={iconStyle} />
                    <Banner>
                      {(data?.lastUsed ?? -1) === -1
                        ? 'never'
                        : timeSince(convertTicks(data.lastUsed))}
                    </Banner>
                  </MetadataItem>
                </Card>
              </div>
            </div>
          </>
        )}
      </section>
      {status === 'success' && data && (
        <>
          <section className="mx-auto max-w-5xl p-6 md:col-span-2">
            <Card title="Key Notes">
              <EditableText
                text={data?.notes}
                pattern={data?.pattern}
                onChange={mutateNotes}
              />
            </Card>
          </section>
          <section className="mx-auto mb-4 max-w-5xl p-6 md:col-span-2">
            <Card title="Danger Zone" danger>
              <div className="flex justify-center gap-4 p-6">
                {!data.flags.disabled && (
                  <Button
                    onClick={pauseKey}
                    appearance={Button.Appearances.solid}
                    color={Button.Colors.danger}
                    size={Button.Sizes.lg}
                    className="group"
                  >
                    <PauseCircleIcon
                      title="pause key"
                      className="mr-2 h-6 cursor-pointer text-sky-100 group-hover:text-sky-400"
                      onClick={pauseKey}
                    />
                    Pause Key
                  </Button>
                )}
                {data.flags.disabled && (
                  <Button
                    onClick={resumeKey}
                    appearance={Button.Appearances.solid}
                    color={Button.Colors.success}
                    size={Button.Sizes.lg}
                    className="group"
                  >
                    <PlayCircleIcon
                      title="resume key"
                      className="mr-2 h-6 cursor-pointer text-emerald-100 group-hover:text-emerald-400"
                      onClick={resumeKey}
                    />{' '}
                    Resume Key
                  </Button>
                )}
                <Button
                  onClick={deleteKey}
                  appearance={Button.Appearances.solid}
                  color={Button.Colors.danger}
                  size={Button.Sizes.lg}
                  className="group"
                >
                  <TrashIcon
                    title="delete key"
                    aria-label="delete site"
                    className="mr-2 h-6 cursor-pointer text-red-100 group-hover:text-red-400"
                    onClick={deleteKey}
                  />
                  Delete Key
                </Button>
              </div>
            </Card>
          </section>
        </>
      )}
    </>
  );
};
Component.displayName = 'Key';

export const ErrorBoundary = () => {
  const { key } = useParams();

  return (
    <>
      <section className="mx-auto flex max-w-5xl gap-4 p-6 md:col-span-2">
        <KeyIcon className="h-14 fill-secondary-500/20 text-primary-500/80 drop-shadow-md dark:fill-primary-500/50 dark:text-secondary-400/80" />
        <div>
          <h2
            id="key-creation"
            className="uppercase text-primary-600 dark:text-primary-200"
          >
            {key}
          </h2>
          <p className="text-primary-400">key does not exist</p>
        </div>
      </section>
      <section className="relative mb-12 w-full px-6 md:mx-auto">
        <div className="bg-circuit absolute inset-0 h-64 bg-primary-600 shadow-lg"></div>
        <div className="mx-auto w-full">
          <div className="mt-16 flex flex-col items-center gap-2">
            <h3 className="mt-16 text-center text-5xl font-black tracking-tight text-secondary-400 drop-shadow-md">
              This key does not exist!
            </h3>
            <ShieldExclamationIcon className="mb-14 h-24 fill-primary-500/70 text-secondary-400/90 drop-shadow-md" />
          </div>
        </div>
      </section>
      <section className="mx-auto flex max-w-5xl justify-center gap-4 p-6">
        <RouterButtonLink
          to="/self-service/keys"
          appearance={Button.Appearances.solid}
          color={Button.Colors.primary}
          size={Button.Sizes.xl}
        >
          Go back to your keys
        </RouterButtonLink>
      </section>
    </>
  );
};
ErrorBoundary.displayName = 'KeyErrorBoundary';

const MetadataItem = ({ children }) => (
  <div className="flex w-[250px] flex-col justify-center gap-4">{children}</div>
);
MetadataItem.propTypes = {
  children: PropTypes.node.isRequired,
};

const Banner = ({ children }) => (
  <div className="flex h-12 w-[250px] items-center justify-center border-y border-y-slate-300 bg-slate-200 px-2 dark:border-y-slate-500/50 dark:bg-slate-600 dark:text-primary-100">
    <p className="overflow-hidden text-ellipsis">{children}</p>
  </div>
);
Banner.displayName = 'Banner';
Banner.propTypes = {
  children: PropTypes.node,
};
