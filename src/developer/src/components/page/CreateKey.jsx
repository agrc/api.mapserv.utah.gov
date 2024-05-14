import { ClipboardIcon } from '@heroicons/react/24/outline';
import { zodResolver } from '@hookform/resolvers/zod';
import * as Tabs from '@radix-ui/react-tabs';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { httpsCallable } from 'firebase/functions';
import { useEffect } from 'react';
import { CopyToClipboard } from 'react-copy-to-clipboard';
import { Controller, useForm } from 'react-hook-form';
import { useLoaderData } from 'react-router-dom';
import { useFunctions } from 'reactfire';
import * as z from 'zod';
import { TextLink } from '../Link';
import Pill from '../Pill';
import Button from '../design-system/Button';
import Card from '../design-system/Card';
import { FormError, FormErrors } from '../design-system/Form';
import Input from '../design-system/Input';
import RadioGroup from '../design-system/RadioGroup';
import Spinner from '../design-system/Spinner';
import TextArea from '../design-system/TextArea';

const items = [
  { label: 'Production', value: 'production' },
  {
    label: 'Development',
    value: 'development',
  },
];

const base = z.object({
  mode: z.enum(['development', 'production']),
  notes: z
    .string()
    .max(500, 'Limit your notes to 500 characters or less')
    .optional(),
});

const privateIps = ['10', '127', '192.168'];
const schema = z.discriminatedUnion('type', [
  z
    .object({
      type: z.literal('server'),
      ip: z
        .string()
        .ip({ version: 'v4' })
        .superRefine((val, ctx) => {
          const firstOctet = val.indexOf('.');

          if (firstOctet === -1) {
            return;
          }

          const value = val.slice(0, firstOctet);

          if (privateIps.includes(value)) {
            ctx.addIssue({
              code: z.ZodIssueCode.custom,
              message: 'This is a private address. Use a public address.',
            });
          }
        }),
    })

    .merge(base),
  z
    .object({
      type: z.literal('browser'),
      pattern: z.string(),
    })
    .merge(base),
]);

const defaultValues = {
  pattern: '',
  notes: '',
  mode: 'development',
  type: 'browser',
  fulfilled: false,
};

export function Component() {
  const {
    control,
    formState: { errors, isSubmitSuccessful },
    handleSubmit,
    reset,
    setValue,
  } = useForm({
    resolver: zodResolver(schema),
    mode: 'onBlur',
    defaultValues,
  });

  const loaderData = useLoaderData();
  const functions = useFunctions();
  const createKey = httpsCallable(functions, 'createKey');
  const getKeys = httpsCallable(functions, 'keys');

  const queryClient = useQueryClient();
  const {
    data,
    mutate,
    status: mutationStatus,
  } = useMutation({
    mutationFn: (data) => Spinner.minDelay(createKey(data)),
    onSuccess: async () => {
      await queryClient.cancelQueries({
        queryKey: ['my keys'],
      });

      await queryClient.refetchQueries({
        queryKey: ['my keys'],
        queryFn: () => getKeys(),
      });
    },
  });

  useEffect(() => {
    if (isSubmitSuccessful) {
      reset(defaultValues);
    }
  }, [isSubmitSuccessful, reset]);

  const onSubmit = (data) =>
    mutate({
      ...data,
      for: loaderData.user.uid,
    });

  return (
    <>
      <section className="border-b border-slate-400 p-6">
        <h2
          id="key-creation"
          className="mx-auto mb-4 max-w-5xl text-wavy-800 dark:text-slate-200 md:col-span-2"
        >
          Key creation
        </h2>
        <div className="mx-auto grid max-w-5xl grid-cols-1 gap-4 md:grid-cols-2 md:gap-10 md:px-6">
          <p className="text-wavy-800 dark:text-slate-200">
            Each key is specific to an application you have created; either a
            browser or server based application. Browser based applications run
            in a web browser. For example, the React geocoding component{' '}
            <TextLink href="https://github.com/agrc/kitchen-sink/tree/main/packages/dart-board">
              dartboard
            </TextLink>
            , running on{' '}
            <TextLink href="https://atlas.utah.gov">atlas.utah.gov</TextLink> is
            a browser based application. The request to the UGRC API is created
            in javascript running inside the browser using the browser&apos;s
            fetch API or with an XHR request.
          </p>
          <p className="text-wavy-800 dark:text-slate-200">
            Server based applications run on a computer or a server. For
            example, the{' '}
            <TextLink href="https://gis.utah.gov/products/sgid/address/api-client/">
              API Client
            </TextLink>{' '}
            is running on your desktop. The request to the UGRC API is called
            directly or indirectly from a server side programming language or
            scripting language like Python, Java, or C#.
          </p>
        </div>
      </section>
      <section className="relative w-full">
        <div className="bg-circuit absolute inset-0 h-64 bg-wavy-600 shadow-lg"></div>
        <div className="relative z-10 mx-auto max-w-5xl px-6">
          <h3 className="mb-3 ml-2 pt-3 text-center text-white md:col-span-2">
            Choosing the key type
          </h3>
          <div className="md:grid md:grid-cols-2 md:gap-10">
            <Card
              title="Browser"
              subTitle="Requests are made by JavaScript running in a browser"
            >
              <div className="flex flex-wrap px-5">
                <Pill>XHR request</Pill>
                <Pill>window.fetch</Pill>
                <Pill>jquery.ajax</Pill>
                <Pill>ArcGIS Maps SDK for JavaScript</Pill>
                <Pill>vue</Pill>
                <Pill>react</Pill>
                <Pill>angular</Pill>
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
        <h3
          id="create-key"
          className="col-span-2 mb-3 ml-2 px-6 text-center text-wavy-800 dark:text-slate-200"
        >
          Create a key
        </h3>
        <div className="mx-auto flex max-w-5xl flex-col gap-6 px-6 ">
          <form
            onSubmit={handleSubmit(onSubmit)}
            className="w-full border border-slate-300 bg-slate-100 shadow-md dark:bg-slate-600"
          >
            <Tabs.Root
              defaultValue="browser"
              onValueChange={(type) => setValue('type', type)}
            >
              <Tabs.List className="grid h-10 w-full grid-cols-2 items-center justify-center p-1">
                <Tabs.Trigger
                  className="relative border-slate-400 py-2 text-lg font-bold after:absolute after:left-0 after:block after:w-full after:rounded-full data-[state=active]:text-wavy-600 data-[state=inactive]:text-slate-500 data-[state=inactive]:after:bottom-1 data-[state=active]:after:h-2 data-[state=inactive]:after:h-px data-[state=active]:after:bg-wavy-400 data-[state=inactive]:after:bg-slate-400 dark:text-slate-200 data-[state=active]:dark:text-mustard-400 data-[state=active]:after:dark:bg-mustard-400 data-[state=inactive]:after:dark:bg-slate-400"
                  value="browser"
                >
                  <div className="mb-2">
                    <span className="rounded-full px-3 py-1 uppercase hover:bg-white hover:dark:bg-slate-500">
                      Browser
                    </span>
                  </div>
                </Tabs.Trigger>
                <Tabs.Trigger
                  className="relative border-slate-400 py-2 text-lg font-bold after:absolute after:left-0 after:block after:w-full after:rounded-full data-[state=active]:text-wavy-600 data-[state=inactive]:text-slate-500 data-[state=inactive]:after:bottom-1 data-[state=active]:after:h-2 data-[state=inactive]:after:h-px data-[state=active]:after:bg-wavy-400 data-[state=inactive]:after:bg-slate-400 dark:text-slate-200 data-[state=active]:dark:text-mustard-400 data-[state=active]:after:dark:bg-mustard-400 data-[state=inactive]:after:dark:bg-slate-400"
                  value="server"
                >
                  <div className="mb-2">
                    <span className="rounded-full px-3 py-1 uppercase hover:bg-white hover:dark:bg-slate-500">
                      Server
                    </span>
                  </div>
                </Tabs.Trigger>
              </Tabs.List>
              <Tabs.Content className="p-8" value="browser">
                <div className="grid grid-cols-2 items-start gap-8 dark:text-slate-200">
                  <Controller
                    name="pattern"
                    control={control}
                    render={({ field }) => (
                      <Input
                        label="URL Pattern"
                        placeholder="*.example.com/*"
                        error={errors.pattern?.message}
                        required
                        {...field}
                      />
                    )}
                  />
                  <Controller
                    name="mode"
                    control={control}
                    render={({ field }) => (
                      <RadioGroup
                        label="Key environment configuration"
                        ariaLabel="Key environment configuration"
                        required
                        items={items}
                        defaultValue="development"
                        {...field}
                      />
                    )}
                  />
                  <Controller
                    name="notes"
                    control={control}
                    render={({ field }) => (
                      <TextArea
                        className="col-span-2"
                        label="Notes"
                        error={errors.notes?.message}
                        placeholder="This key will be used for..."
                        {...field}
                      />
                    )}
                  />
                </div>
              </Tabs.Content>
              <Tabs.Content className="p-8" value="server">
                <FormErrors errors={errors} />
                <div className="grid grid-cols-2 items-start gap-8 dark:text-slate-200">
                  <Controller
                    name="ip"
                    control={control}
                    render={({ field }) => (
                      <Input
                        label="IP Address"
                        placeholder="10.0.0.1"
                        required
                        error={errors.ip?.message}
                        {...field}
                      />
                    )}
                  />
                  <Controller
                    name="mode"
                    control={control}
                    render={({ field }) => (
                      <RadioGroup
                        label="Key environment configuration"
                        ariaLabel="Key environment configuration"
                        required
                        items={items}
                        defaultValue="development"
                        {...field}
                      />
                    )}
                  />
                  <Controller
                    name="notes"
                    control={control}
                    render={({ field }) => (
                      <TextArea
                        className="col-span-2"
                        label="Notes"
                        error={errors.notes?.message}
                        placeholder="This key will be used for..."
                        {...field}
                      />
                    )}
                  />
                </div>
              </Tabs.Content>
            </Tabs.Root>
            {mutationStatus === 'loading' && (
              <div className="relative mx-auto mb-12 flex w-full items-center justify-center gap-6 border border-x-0 py-4 text-2xl font-black shadow dark:bg-slate-500 dark:text-mustard-200 md:w-3/4 md:border-x md:text-4xl">
                <Spinner
                  size={Spinner.Sizes.custom}
                  className="h-8"
                  ariaLabel="waiting to generate key"
                />{' '}
                Creating key...
              </div>
            )}
            {mutationStatus === 'success' && (
              <div className="relative mx-auto mb-12 flex w-full items-center justify-center gap-6 border border-x-0 border-wavy-400/70 bg-slate-300/70 py-4 text-2xl font-black uppercase text-wavy-500 shadow dark:bg-slate-500 dark:text-mustard-200 md:w-3/4 md:border-x md:text-4xl">
                {data.data}
                <CopyToClipboard
                  text={data.data}
                  className="absolute right-1 top-1 h-8 cursor-pointer hover:text-wavy-400 dark:hover:text-mustard-500"
                >
                  <ClipboardIcon title="copy to clipboard" />
                </CopyToClipboard>
              </div>
            )}
            {mutationStatus === 'error' && (
              <FormError>
                <span>
                  We had some trouble creating this key. Give it another try and
                  if it fails again, create an issue in{' '}
                  <TextLink href="https://github.com/agrc/api.mapserv.utah.gov/issues/new">
                    GitHub
                  </TextLink>{' '}
                  or tweet us{' '}
                  <TextLink href="https://twitter.com/maputah">
                    @MapUtah
                  </TextLink>
                  .
                </span>
              </FormError>
            )}
            <div className="flex justify-center gap-6 pb-6">
              <Button
                type={Button.Types.submit}
                appearance={Button.Appearances.solid}
                color={Button.Colors.primary}
                size={Button.Sizes.xl}
                disabled={mutationStatus === 'pending'}
                busy={mutationStatus === 'pending'}
              >
                create key
              </Button>
            </div>
          </form>
          <p className="text-wavy-800 dark:text-slate-200">
            If you need help choosing the type or the value please visit and
            read the{' '}
            <TextLink href="https://ut-dts-agrc-web-api-dev.web.app/getting-started/">
              getting started guide
            </TextLink>
            .
          </p>
        </div>
      </section>
    </>
  );
}
Component.displayName = 'CreateKey';
