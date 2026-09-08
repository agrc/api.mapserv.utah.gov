import { useQuery } from '@tanstack/react-query';
import { legacyCreateColumnHelper } from '@tanstack/react-table/legacy';
import { Button } from '@ugrc/utah-design-system/components/Button';
import { Spinner } from '@ugrc/utah-design-system/components/Spinner';
import { useFirebaseFunctions } from '@ugrc/utah-design-system/contexts/FirebaseFunctionsProvider';
import { httpsCallable } from 'firebase/functions';
import { Link, useLoaderData, useNavigate } from 'react-router';
import CopyToClipboard from '../CopyToClipboard';
import Table from '../design-system/Table';

const columnHelper = legacyCreateColumnHelper();
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
              className="text-secondary-800 dark:text-secondary-300"
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
    header: () => <span className="sr-only">Actions</span>,
    cell: (info) => (
      <div className="flex justify-end">
        <CopyToClipboard text={info.row.getValue('key')} className="h-6" />
      </div>
    ),
  }),
];

export function Component() {
  const { functions } = useFirebaseFunctions();
  const getKeys = httpsCallable(functions, 'keys');
  const loaderData = useLoaderData();
  const navigate = useNavigate();

  const { status, data } = useQuery({
    queryKey: ['my keys', loaderData.uid],
    queryFn: () => Spinner.minDelay(getKeys()),
    enabled: (loaderData?.uid.length ?? 0 > 0) ? true : false,
    onError: () => 'We had some trouble finding your keys.',
    gcTime: Infinity,
    staleTime: Infinity,
  });

  return (
    <>
      <section className="border-b border-slate-400 p-6">
        <div className="mx-auto max-w-5xl md:px-6">
          <h2 id="my-keys" className="text-primary-800 md:col-span-2 dark:text-slate-100">
            Manage keys
          </h2>
          <p className="text-primary-900 mt-4 dark:text-slate-100">
            API keys are used to authenticate requests to the UGRC API. You can create as many keys as you need and each
            key is associated with an application or a process. Here you can manage your keys by deleting or pausing
            them. You can add notes to help you remember what they are used for. And you can see analytics about how
            much they are used.
          </p>
        </div>
        <div className="mt-6 flex justify-center gap-6">
          <Button onPress={() => navigate('/self-service/create-key')} size="large">
            create a new key
          </Button>
          <Button onPress={() => navigate('/self-service/claim-account')} size="large">
            claim a non-Utahid key
          </Button>
        </div>
      </section>
      <section className="mb-12 w-full">
        <div className="bg-circuit bg-primary-600 w-full shadow-lg">
          <div className="px-6 pt-12 pb-12"></div>
        </div>
        {status === 'pending' ? (
          <div className="border-primary-500/50 relative mx-auto -mt-8 flex min-h-62.5 max-w-5xl flex-col items-center justify-center border-2 border-b border-b-slate-300 bg-white px-6 shadow-md dark:border-slate-500/50 dark:bg-slate-800">
            <span className="text-primary-400 size-16">
              <Spinner ariaLabel="fetching API keys" />
            </span>
          </div>
        ) : (
          <Table
            columns={columns}
            data={data?.data ?? []}
            visibility={{ createdDate: false }}
            className="border-primary-500/50 dark:border-secondary-500/30 mx-auto -mt-8 min-h-62.5 max-w-5xl border-2 bg-white text-sm shadow-md dark:border dark:bg-slate-800"
            caption="Your API keys"
          />
        )}
      </section>
    </>
  );
}
Component.displayName = 'Keys';
