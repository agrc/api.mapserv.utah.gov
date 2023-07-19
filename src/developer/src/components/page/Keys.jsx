import {
  InformationCircleIcon,
  PauseCircleIcon,
  PlayCircleIcon,
  TrashIcon,
} from '@heroicons/react/20/solid';
import { useQuery } from '@tanstack/react-query';
import { createColumnHelper } from '@tanstack/react-table';
import { httpsCallable } from 'firebase/functions';
import { Link, useLoaderData } from 'react-router-dom';
import { useFunctions } from 'reactfire';
import { RouterButtonLink } from '../design-system/Button';
import Spinner from '../design-system/Spinner';
import Table from '../design-system/Table';

const columnHelper = createColumnHelper({ enableHiding: true });
const columns = [
  columnHelper.accessor('key', {
    header: () => <span>Key</span>,
    cell: (info) => {
      const value = info.getValue();
      return (
        <Link className="" to={`${value}`}>
          {value}
        </Link>
      );
    },
  }),
  columnHelper.accessor('notes', {
    header: () => <span>Notes</span>,
    cell: (info) => {
      const value = info.getValue();
      if (value === '[api-client]') {
        return (
          <>
            Visit{' '}
            <a
              className="text-mustard-700"
              href="https://gis.utah.gov/data/address-geocoders-locators/#OfficialClient"
            >
              gis.utah.gov
            </a>{' '}
            to get started with the UGRC API Client
          </>
        );
      }

      return value;
    },
  }),
  columnHelper.accessor('createdDate', {}),
  columnHelper.accessor('created', {
    header: () => <span>Created</span>,
    cell: (info) => (
      <span className="cursor-default" title={info.row.getValue('createdDate')}>
        {info.getValue()}
      </span>
    ),
  }),
  // columnHelper.accessor('lastUsed', {
  //   header: () => <span>Last Used</span>,
  //   cell: (info) => info.getValue(),
  // }),
  columnHelper.accessor('action', {
    header: null,
    cell: (info) => {
      return (
        <div className="flex justify-end">
          <TrashIcon
            aria-label="delete site"
            className="h-6 cursor-pointer text-red-600 hover:text-red-900"
            onClick={(event) => {
              event.stopPropagation();

              console.log(info.row.getValue('key'));
            }}
          />
          <PauseCircleIcon
            className="h-6 cursor-pointer text-sky-600 hover:text-sky-900"
            onClick={(event) => {
              event.stopPropagation();

              console.log(info.row.getValue('key'));
            }}
          />
          <PlayCircleIcon
            className="h-6 cursor-pointer text-emerald-600 hover:text-emerald-900"
            onClick={(event) => {
              event.stopPropagation();

              console.log(info.row.getValue('key'));
            }}
          />
          <InformationCircleIcon
            className="h-6 cursor-pointer text-fuchsia-600 hover:text-fuchsia-900"
            onClick={(event) => {
              event.stopPropagation();

              console.log(info.row.getValue('key'));
            }}
          />
        </div>
      );
    },
  }),
];

export function Component() {
  const functions = useFunctions();
  const getKeys = httpsCallable(functions, 'keys');
  const loaderData = useLoaderData();

  const { status, data } = useQuery({
    queryKey: ['my keys', loaderData.user.uid],
    queryFn: async () => await Spinner.minDelay(getKeys(), 1000),
    enabled: loaderData.user?.uid.length ?? 0 > 0 ? true : false,
    onError: () => 'We had some trouble finding your contacts.',
    cacheTime: Infinity,
  });

  return (
    <>
      <section className="border-b border-slate-400 p-6">
        <div className="mx-auto max-w-5xl md:px-6">
          <h2
            id="my-keys"
            className="md:col-span-2 text-wavy-800 dark:text-slate-200"
          >
            Manage Keys
          </h2>
          <p className="text-wavy-800 dark:text-slate-200 mt-4">
            API keys are used to authenticate requests to the UGRC API. You can
            create as many keys as you need and each key is associated with an
            application or a process. Here you can manage your keys by deleting
            or pausing them. You can add notes to help you remember what they
            are used for. And you can see analytics about how much they are
            used.
          </p>
        </div>
        <div className="flex justify-center gap-6 mt-6">
          <RouterButtonLink
            to="/self-service/create-key"
            appearance="solid"
            color="primary"
            size="xl"
          >
            create a new key
          </RouterButtonLink>
        </div>
      </section>
      <section className="relative w-full md:mx-auto mb-12 px-6">
        <div className="bg-circuit absolute inset-0 h-64 bg-wavy-600 shadow-lg"></div>
        <div className="pt-12"></div>
        {status === 'loading' ? (
          <div className="justify-center items-center relative flex flex-col border-b border-b-slate-300 bg-white dark:bg-slate-800 min-h-[250px] max-w-5xl mx-auto border-wavy-500/50 border-2 dark:border-slate-500/50 shadow-md">
            <Spinner
              size={Spinner.Sizes.custom}
              className="h-16 text-wavy-400"
              ariaLabel="fetching API keys"
            />
          </div>
        ) : (
          <Table
            columns={columns}
            data={data.data}
            visibility={{ createdDate: false }}
            className="bg-white dark:bg-slate-800 min-h-[250px] max-w-5xl mx-auto border-wavy-500/50 border-2 dark:border-slate-500/50 shadow-md text-sm"
            caption="Your API keys"
          />
        )}
      </section>
    </>
  );
}
Component.displayName = 'Keys';
