import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Button,
  ExternalLink,
  FormError,
  FormErrors,
  Radio,
  RadioGroup,
  Spinner,
  Tab,
  TabList,
  TabPanel,
  Tabs,
  TextArea,
  TextField,
  useFirebaseFunctions,
} from '@ugrc/utah-design-system';
import { httpsCallable } from 'firebase/functions';
import { Controller, useForm } from 'react-hook-form';
import { useLoaderData, useNavigate } from 'react-router';
import * as z from 'zod';
import CopyToClipboard from '../CopyToClipboard';
import Card from '../design-system/Card';
import Pill from '../Pill';

const base = z.object({
  mode: z.enum(['development', 'production']),
  notes: z.string().max(500, 'Limit your notes to 500 characters or less').optional(),
});

const privateIps = ['10', '127', '192.168'];

const schema = z.discriminatedUnion('type', [
  z.object({
    type: z.literal('server'),
    ...base.shape,
    ip: z.ipv4().superRefine((val, ctx) => {
      const firstOctet = val.slice(0, val.indexOf('.'));

      if (privateIps.includes(firstOctet)) {
        ctx.addIssue({
          message: 'This is a private address. Use a public address.',
        });
      }
    }),
  }),
  z.object({
    type: z.literal('browser'),
    ...base.shape,
    pattern: z.string().min(1, 'A URL pattern is required'),
  }),
]);

const defaultValues = {
  ip: '',
  pattern: '',
  notes: '',
  mode: 'development',
  type: 'browser',
};

const displayError = (error) => {
  return error.code === 'functions/already-exists' ? (
    <FormError>
      <span>
        This key has already been created ({error.details}). Please reuse this key or create a key with unique
        information.
      </span>
    </FormError>
  ) : (
    <FormError>
      <span>
        We had some trouble creating this key. Give it another try and if it fails again, create an issue in{' '}
        <ExternalLink href="https://github.com/agrc/api.mapserv.utah.gov/issues/new">GitHub</ExternalLink> or tweet us{' '}
        <ExternalLink href="https://x.com/maputah">@MapUtah</ExternalLink>.
      </span>
    </FormError>
  );
};

export function Component() {
  const {
    control,
    formState: { errors },
    handleSubmit,
    reset,
    setValue,
  } = useForm({
    resolver: zodResolver(schema),
    mode: 'onBlur',
    defaultValues,
    shouldUnregister: true,
  });

  const loaderData = useLoaderData();
  const navigate = useNavigate();
  const { functions } = useFirebaseFunctions();
  const createKey = httpsCallable(functions, 'createKey');
  const getKeys = httpsCallable(functions, 'keys');

  const queryClient = useQueryClient();
  const {
    data,
    error,
    mutate,
    status: mutationStatus,
  } = useMutation({
    mutationKey: ['create key', loaderData.uid],
    mutationFn: (data) => Spinner.minDelay(createKey(data)),
    onSuccess: async () => {
      reset(defaultValues);

      await queryClient.cancelQueries({
        queryKey: ['my keys'],
      });

      await queryClient.refetchQueries({
        queryKey: ['my keys'],
        queryFn: () => getKeys(),
      });
    },
  });

  const onSubmit = (data) =>
    mutate({
      ...data,
      for: loaderData.uid,
    });

  return (
    <>
      <section className="border-b border-slate-400 p-6">
        <h2 id="key-creation" className="mx-auto mb-4 max-w-5xl text-primary-800 dark:text-slate-200 md:col-span-2">
          Key creation
        </h2>
        <div className="mx-auto grid max-w-5xl grid-cols-1 gap-4 md:grid-cols-2 md:gap-10 md:px-6">
          <p className="text-primary-900 dark:text-slate-100">
            Each key is specific to an application you have created; either a browser or server based application.
            Browser based applications run in a web browser. For example, the React Geocoding component from the{' '}
            <ExternalLink href="https://www.npmjs.com/package/@ugrc/utah-design-system/">
              Utah Design System
            </ExternalLink>
            , running in <ExternalLink href="https://atlas.utah.gov">atlas.utah.gov</ExternalLink> is a browser based
            application. The request to the UGRC API is created in javascript running inside the browser using the
            browser&apos;s fetch API or with an XHR request.
          </p>
          <p className="text-primary-900 dark:text-slate-100">
            Server based applications run on a desktop or server computer. For example, the{' '}
            <ExternalLink href="https://gis.utah.gov/products/sgid/address/api-client/">API Client</ExternalLink> is
            running on your desktop computer. The request to the UGRC API is called directly or indirectly from a server
            side programming language or scripting language like Python, Java, or C#.
          </p>
        </div>
      </section>
      <section className="relative w-full">
        <div className="bg-circuit absolute inset-0 h-64 bg-primary-600 shadow-lg"></div>
        <div className="relative z-10 mx-auto max-w-5xl px-6">
          <h3 className="mb-3 ml-2 pt-3 text-center text-white md:col-span-2">Choosing the key type</h3>
          <div className="md:grid md:grid-cols-2 md:gap-10">
            <Card title="Browser" subTitle="Requests are made by JavaScript running in a browser">
              <div className="flex flex-wrap px-5">
                <Pill>XHR request</Pill>
                <Pill>fetch</Pill>
                <Pill>ky</Pill>
                <Pill>Axios</Pill>
                <Pill>ArcGIS Maps SDK for JavaScript</Pill>
                <Pill>AngularJS HttpClient</Pill>
                <Pill>TanStack Query</Pill>
              </div>
            </Card>
            <Card
              title="Server"
              subTitle="Requests are made by code or a tool executing on a computer or a
                server"
            >
              <div className="flex flex-wrap px-5">
                <Pill>ArcPy</Pill>
                <Pill>Cloud Run</Pill>
                <Pill>ArcGIS Pro</Pill>
                <Pill>Python</Pill>
                <Pill>Java</Pill>
                <Pill>C#</Pill>
                <Pill>Node.js</Pill>
                <Pill>UGRC API Client</Pill>
              </div>
            </Card>
          </div>
        </div>
      </section>
      <section className="mb-12 mt-6 max-w-5xl md:mx-auto">
        <h3 id="create-key" className="col-span-2 mb-3 ml-2 px-6 text-center text-primary-800 dark:text-slate-200">
          Create a key
        </h3>
        <div className="mx-auto flex max-w-5xl flex-col gap-6 px-6">
          <form
            onSubmit={handleSubmit(onSubmit)}
            className="w-full border border-slate-300 bg-slate-100 shadow-md dark:border-slate-800 dark:bg-slate-700"
          >
            <FormErrors errors={errors} />
            <Tabs
              className="grid-grid-cols-2 p-2"
              onSelectionChange={(value) => {
                setValue('type', value);
              }}
            >
              <TabList aria-label="Key type" className="justify-around">
                <Tab id="browser" className="justify-center uppercase">
                  Browser
                </Tab>
                <Tab id="server" className="justify-center uppercase">
                  Server
                </Tab>
              </TabList>
              <TabPanel className="grid grid-cols-2 items-start gap-8 p-8 dark:text-slate-200" id="browser">
                <Controller
                  name="pattern"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      label="URL Pattern"
                      placeholder="*.example.com/*"
                      isRequired
                      {...field}
                      onChange={(value) => field.onChange(value.trim())}
                    />
                  )}
                />
                <Controller
                  name="mode"
                  control={control}
                  render={({ field: { value, onBlur, onChange } }) => (
                    <RadioGroup
                      label="Key environment configuration"
                      value={value}
                      orientation="horizontal"
                      onBlur={onBlur}
                      onChange={onChange}
                    >
                      <Radio value="development">Development</Radio>
                      <Radio value="production">Production</Radio>
                    </RadioGroup>
                  )}
                />
                <Controller
                  name="notes"
                  control={control}
                  render={({ field }) => (
                    <TextArea
                      className="col-span-2"
                      label="Notes"
                      placeholder="This key will be used for..."
                      {...field}
                    />
                  )}
                />
              </TabPanel>
              <TabPanel className="grid grid-cols-2 items-start gap-8 p-8 dark:text-slate-200" id="server">
                <Controller
                  name="ip"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      label="IP Address"
                      placeholder="198.51.100.1"
                      isRequired
                      {...field}
                      onChange={(value) => field.onChange(value.trim())}
                    />
                  )}
                />
                <Controller
                  name="mode"
                  control={control}
                  render={({ field: { value, onBlur, onChange } }) => (
                    <RadioGroup
                      label="Key environment configuration"
                      orientation="horizontal"
                      value={value}
                      onBlur={onBlur}
                      onChange={onChange}
                    >
                      <Radio value="development">Development</Radio>
                      <Radio value="production">Production</Radio>
                    </RadioGroup>
                  )}
                />
                <Controller
                  name="notes"
                  control={control}
                  render={({ field }) => (
                    <TextArea
                      className="col-span-2"
                      label="Notes"
                      placeholder="This key will be used for..."
                      {...field}
                    />
                  )}
                />
              </TabPanel>
            </Tabs>
            {mutationStatus === 'pending' && (
              <div className="relative mx-auto mb-12 flex w-full items-center justify-center gap-6 border border-x-0 py-4 text-2xl font-black text-primary-500 shadow dark:bg-slate-500 dark:text-secondary-200 md:w-3/4 md:border-x md:text-4xl">
                Creating key...
              </div>
            )}
            {mutationStatus === 'success' && (
              <div className="relative mx-auto mb-12 flex w-full items-center justify-center gap-6 border border-x-0 border-primary-400/70 bg-slate-300/70 py-4 text-2xl font-black uppercase text-primary-500 shadow dark:bg-slate-500 dark:text-secondary-200 md:w-3/4 md:border-x md:text-4xl">
                <Button
                  onPress={() => navigate(`/self-service/keys/${data.data}`)}
                  className="absolute left-1 top-1 font-normal"
                  size="extraSmall"
                  variant="accent"
                >
                  details
                </Button>
                {data.data}
                <CopyToClipboard text={data.data} className="absolute right-1 top-1" />
              </div>
            )}
            {mutationStatus === 'error' && displayError(error)}
            <div className="flex justify-center gap-6 pb-6">
              <Button type="submit" size="extraLarge" isPending={mutationStatus === 'pending'}>
                create key
              </Button>
              <Button onPress={() => navigate('/self-service/keys')} variant="secondary" size="extraLarge">
                manage keys
              </Button>
            </div>
          </form>
          <p className="text-primary-900 dark:text-slate-100">
            If you need help choosing the key type or the value associated with the key, please visit and read the{' '}
            <ExternalLink href={`${import.meta.env.VITE_API_EXPLORER_URL}/getting-started/`}>
              getting started guide
            </ExternalLink>
            .
          </p>
        </div>
      </section>
    </>
  );
}
Component.displayName = 'CreateKey';
