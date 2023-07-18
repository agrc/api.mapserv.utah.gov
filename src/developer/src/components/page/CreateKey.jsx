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

const items = [
  { label: 'Production', value: 'production' },
  {
    label: 'Development',
    value: 'development',
  },
];

const base = z.object({
  mode: z.enum(['development', 'production']),
});

const privateIps = [10, 127, 172, 192];
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
  mode: 'development',
  type: 'browser',
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
    mode: 'all',
    defaultValues,
  });

  const loaderData = useLoaderData();
  const functions = useFunctions();
  const createKey = httpsCallable(functions, 'createKey');

  const queryClient = useQueryClient();
  const {
    data,
    mutate,
    status: mutationStatus,
  } = useMutation({
    mutationFn: (data) => Spinner.minDelay(createKey(data), 5000),
    onSuccess: async () => {
      await queryClient.cancelQueries();

      queryClient.invalidateQueries({ queryKey: ['my keys'] });
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
      fulfilled: false,
    });

  return (
    <>
      <section className="border-b border-slate-400 p-6">
        <div className="mx-auto grid grid-cols-1 md:grid-cols-2 max-w-5xl gap-4 md:gap-10 md:px-6">
          <h2
            id="key-creation"
            className="md:col-span-2 text-wavy-800 dark:text-slate-200"
          >
            Key creation
          </h2>
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
            <TextLink href="https://gis.utah.gov/data/address-geocoders-locators/#OfficialClient">
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
                <Pill>UGRC Geocoding Toolbox</Pill>
                <Pill>UGRC API Client</Pill>
              </div>
            </Card>
          </div>
        </div>
      </section>
      <section className="mt-6 max-w-5xl md:mx-auto mb-12">
        <h3
          id="create-key"
          className="col-span-2 mb-3 ml-2 px-6 text-wavy-800 dark:text-slate-200"
        >
          Create a key
        </h3>
        <div className="mx-auto flex max-w-5xl flex-col gap-6 px-6 ">
          <form
            onSubmit={handleSubmit(onSubmit)}
            className="w-full border border-slate-400 dark:bg-slate-600"
          >
            <Tabs.Root
              defaultValue="browser"
              onValueChange={(type) => setValue('type', type)}
            >
              <Tabs.List className="grid h-10 w-full grid-cols-2 items-center justify-center p-1">
                <Tabs.Trigger
                  className="relative border-slate-400 py-2 text-lg font-bold dark:text-slate-200 data-[state=active]:dark:text-mustard-400 data-[state=inactive]:after:h-px data-[state=active]:after:h-2 data-[state=inactive]:after:dark:bg-slate-400 data-[state=active]:after:dark:bg-mustard-400 data-[state=inactive]:after:bottom-1 after:block after:absolute after:left-0 after:w-full after:rounded-full"
                  value="browser"
                >
                  <span className="rounded-full px-3 py-1 hover:bg-slate-500">
                    Browser
                  </span>
                </Tabs.Trigger>
                <Tabs.Trigger
                  className="relative border-slate-400 py-2 text-lg font-bold dark:text-slate-200 data-[state=active]:dark:text-mustard-400 data-[state=inactive]:after:h-px data-[state=active]:after:h-2 data-[state=inactive]:after:dark:bg-slate-400 data-[state=active]:after:dark:bg-mustard-400 data-[state=inactive]:after:bottom-1 after:block after:absolute after:left-0 after:w-full after:rounded-full"
                  value="server"
                >
                  <span className="rounded-full px-3 py-1 hover:bg-slate-500">
                    Server
                  </span>
                </Tabs.Trigger>
              </Tabs.List>
              <Tabs.Content className="p-8" value="browser">
                <div className="grid grid-cols-2 items-start gap-8 dark:text-slate-200">
                  <Controller
                    name="pattern"
                    control={control}
                    rules={{ required: true }}
                    render={({ field }) => (
                      <Input
                        label="URL Pattern"
                        placeholder="*.example.com/*"
                        required
                        {...field}
                      />
                    )}
                  />
                  <Controller
                    name="mode"
                    control={control}
                    rules={{ required: true }}
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
                </div>
              </Tabs.Content>
              <Tabs.Content className="p-8" value="server">
                <FormErrors errors={errors} />
                <div className="grid grid-cols-2 items-start gap-8 dark:text-slate-200">
                  <Controller
                    name="ip"
                    control={control}
                    rules={{ required: true }}
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
                    rules={{ required: true }}
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
                </div>
              </Tabs.Content>
            </Tabs.Root>
            {mutationStatus === 'loading' && (
              <div className="relative flex items-center justify-center gap-6 py-4 mb-12 border w-full md:w-3/4 mx-auto font-black text-2xl md:text-4xl dark:text-mustard-200 dark:bg-slate-500 shadow border-x-0 md:border-x">
                <Spinner
                  size={Spinner.Sizes.custom}
                  className="h-8"
                  ariaLabel="waiting to generate key"
                />{' '}
                Creating key...
              </div>
            )}
            {mutationStatus === 'success' && (
              <div className="relative flex items-center justify-center gap-6 py-4 mb-12 border w-full md:w-3/4 mx-auto font-black text-2xl md:text-4xl dark:text-mustard-200 dark:bg-slate-500 shadow border-x-0 md:border-x">
                {data.data}
                <CopyToClipboard
                  text={data.data}
                  className="absolute top-1 right-1 h-8 cursor-pointer hover:text-mustard-400 dark:hover:text-mustard-500"
                >
                  <ClipboardIcon title="copy to clipboard" />
                </CopyToClipboard>
              </div>
            )}
            {mutationStatus === 'error' && (
              <FormError message="We had some trouble creating this key. Give it another try and create an issue in GitHub if it fails again. Or tweet us @MapUtah." />
            )}
            <div className="flex justify-center gap-6 pb-6">
              <Button
                type="submit"
                appearance="solid"
                color="primary"
                size="xl"
                disabled={mutationStatus === 'loading'}
              >
                Create Key
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
