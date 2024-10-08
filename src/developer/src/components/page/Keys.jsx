import { useQuery } from '@tanstack/react-query';
import { createColumnHelper } from '@tanstack/react-table';
import { httpsCallable } from 'firebase/functions';
import { Link, useLoaderData } from 'react-router-dom';
import { useFunctions } from 'reactfire';
import CopyToClipboard from '../CopyToClipboard';
import Button, { RouterButtonLink } from '../design-system/Button';
import Spinner from '../design-system/Spinner';
import Table from '../design-system/Table';

const columnHelper = createColumnHelper({ enableHiding: true });
const columns = [
  columnHelper.accessor('key', {
    header: () => <span>Key</span>,
    cell: (info) => {
      const value = info.getValue().toUpperCase();
      return <Link to={`${value.toLowerCase()}`}>{value}</Link>;
    },
  }),
  columnHelper.accessor('notes', {
    header: () => <span>Notes</span>,
    cell: (info) => {
      const value = info.getValue();
      if (value === '[api-client]') {
        return (
          <>
            🚀️ UGRC API Client. Visit{' '}
            <a
              className="text-mustard-700"
              href="https://gis.utah.gov/products/sgid/address/api-client/"
            >
              gis.utah.gov
            </a>{' '}
            to get started! 🚀️
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
    cell: (info) => (
      <div className="flex justify-end">
        <CopyToClipboard text={info.row.getValue('key')} className="h-6" />
      </div>
    ),
  }),
];

export function Component() {
  const functions = useFunctions();
  const getKeys = httpsCallable(functions, 'keys');
  const loaderData = useLoaderData();

  const { status, data } = useQuery({
    queryKey: ['my keys', loaderData.user.uid],
    queryFn: () => Spinner.minDelay(getKeys()),
    enabled: (loaderData.user?.uid.length ?? 0 > 0) ? true : false,
    onError: () => 'We had some trouble finding your keys.',
    gcTime: Infinity,
    staleTime: Infinity,
  });

  return (
    <>
      <section className="border-b border-slate-400 p-6">
        <div className="mx-auto max-w-5xl md:px-6">
          <h2
            id="my-keys"
            className="text-wavy-800 dark:text-slate-200 md:col-span-2"
          >
            Manage keys
          </h2>
          <p className="mt-4 text-wavy-800 dark:text-slate-200">
            API keys are used to authenticate requests to the UGRC API. You can
            create as many keys as you need and each key is associated with an
            application or a process. Here you can manage your keys by deleting
            or pausing them. You can add notes to help you remember what they
            are used for. And you can see analytics about how much they are
            used.
          </p>
        </div>
        <div className="mt-6 flex justify-center gap-6">
          <RouterButtonLink
            to="/self-service/create-key"
            appearance={Button.Appearances.solid}
            color={Button.Colors.primary}
            size={Button.Sizes.xl}
          >
            create a new key
          </RouterButtonLink>
          <RouterButtonLink
            to="/self-service/claim-account"
            appearance={Button.Appearances.solid}
            color={Button.Colors.primary}
            size={Button.Sizes.xl}
          >
            claim a non-Utahid key
          </RouterButtonLink>
        </div>
      </section>
      <section className="relative mb-12 w-full px-6 md:mx-auto">
        <div className="bg-circuit absolute inset-0 h-64 bg-wavy-600 shadow-lg"></div>
        <div className="pt-12"></div>
        {status === 'pending' ? (
          <div className="relative mx-auto flex min-h-[250px] max-w-5xl flex-col items-center justify-center border-2 border-b border-wavy-500/50 border-b-slate-300 bg-white shadow-md dark:border-slate-500/50 dark:bg-slate-800">
            <Spinner
              size={Spinner.Sizes.custom}
              className="h-16 text-wavy-400"
              ariaLabel="fetching API keys"
            />
          </div>
        ) : (
          <Table
            columns={columns}
            data={data?.data ?? []}
            visibility={{ createdDate: false }}
            className="mx-auto min-h-[250px] max-w-5xl border-2 border-wavy-500/50 bg-white text-sm shadow-md dark:border dark:border-mustard-500/30 dark:bg-slate-800"
            caption="Your API keys"
          />
        )}
      </section>
    </>
  );
}
Component.displayName = 'Keys';
