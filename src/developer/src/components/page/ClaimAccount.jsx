import { XMarkIcon } from '@heroicons/react/20/solid';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { httpsCallable } from 'firebase/functions';
import { Controller, useForm } from 'react-hook-form';
import { useFunctions } from 'reactfire';
import * as z from 'zod';
import { TextLink } from '../Link';
import Button, { RouterButtonLink } from '../design-system/Button';
import { FormError, FormErrors } from '../design-system/Form';
import Input from '../design-system/Input';
import Spinner from '../design-system/Spinner';

const schema = z
  .object({
    email: z.string().email(),
    password: z.string(),
    confirm: z.string(),
  })
  .refine((data) => data.password === data.confirm, {
    message: "Passwords don't match",
    path: ['confirm'], // path of error
  });

const defaultValues = {
  email: '',
  password: '',
  confirm: '',
  fulfilled: false,
};

export function Component() {
  const {
    control,
    formState: { errors },
    handleSubmit,
    reset,
  } = useForm({
    resolver: zodResolver(schema),
    mode: 'onBlur',
    defaultValues,
  });

  const functions = useFunctions();
  const validateClaim = httpsCallable(functions, 'validateClaim');

  const queryClient = useQueryClient();
  const {
    data: response,
    mutate,
    reset: resetMutation,
    status: mutationStatus,
  } = useMutation({
    mutationFn: (data) => Spinner.minDelay(validateClaim(data)),
    onSuccess: async (response) => {
      if (response.data.keys.length === 0) {
        return;
      }

      await queryClient.cancelQueries();

      queryClient.invalidateQueries({ queryKey: ['my keys'] });
      reset(defaultValues);
    },
  });

  /**
   * The on submit function for the form which calls the validateClaim function
   * @param {{email: string, password: string}} formData
   */
  const onSubmit = (formData) => mutate(formData);

  return (
    <>
      <section className="border-b border-slate-400 p-6">
        <h2
          id="key-claiming"
          className="mx-auto mb-4 max-w-5xl text-wavy-800 dark:text-slate-200 md:col-span-2"
        >
          Legacy account key claiming
        </h2>
        <div className="mx-auto grid max-w-5xl grid-cols-1 gap-4 md:grid-cols-2 md:gap-10 md:px-6">
          <p className="text-wavy-800 dark:text-slate-200">
            The UGRC API has migrated login systems. Because of this change, you
            currently do not have access to any of the existing keys from your
            legacy account. All of these keys will continue to function for the
            time being. However, if you would like to manage these keys and
            ensure their longevity the keys will need to be claimed from the
            account that originally created them.
          </p>
          <p className="text-wavy-800 dark:text-slate-200">
            In order to do this, you will need to know the email address and
            password of the legacy account that created the keys. You can claim
            keys from multiple legacy accounts if necessary.
          </p>
        </div>
      </section>
      <section className="relative w-full">
        <div className="bg-circuit absolute inset-0 h-64 bg-wavy-600 shadow-lg"></div>
        <div className="relative z-10 mx-auto max-w-5xl px-6">
          <h3 className="mb-3 ml-2 pt-3 text-center text-white md:col-span-2">
            Claim Keys From Account
          </h3>
          <form
            onSubmit={handleSubmit(onSubmit)}
            className="mb-12 grid w-full gap-6 border border-slate-300 bg-slate-100 p-8 shadow-md dark:bg-slate-600"
          >
            <FormErrors errors={errors} />
            <div className="grid grid-cols-1 items-start gap-8 dark:text-slate-200">
              <Controller
                name="email"
                control={control}
                render={({ field }) => (
                  <Input
                    label="legacy account email"
                    placeholder="you@email.com"
                    type="email"
                    error={errors.email?.message}
                    required
                    {...field}
                  />
                )}
              />
              <Controller
                name="password"
                control={control}
                render={({ field }) => (
                  <Input
                    label="legacy account password"
                    type="password"
                    error={errors.password?.message}
                    required
                    {...field}
                  />
                )}
              />
              <Controller
                name="confirm"
                control={control}
                render={({ field }) => (
                  <Input
                    label="confirm"
                    type="password"
                    error={errors.confirm?.message}
                    required
                    {...field}
                  />
                )}
              />
            </div>

            {mutationStatus === 'loading' && (
              <div className="relative mx-auto mb-12 flex w-full items-center justify-center gap-6 border border-x-0 py-4 text-2xl font-black shadow dark:bg-slate-500 dark:text-mustard-200 md:w-3/4 md:border-x md:text-4xl">
                <Spinner
                  size={Spinner.Sizes.custom}
                  className="h-8"
                  ariaLabel="waiting to claim account"
                />{' '}
                Claiming keys from account...
              </div>
            )}
            {mutationStatus === 'success' && (
              <div className="relative mx-auto flex w-full flex-col items-center justify-center gap-2 border border-x-0 border-wavy-400/70 bg-slate-300/70 px-6 py-4 text-2xl font-black uppercase text-wavy-500 shadow dark:bg-slate-500 dark:text-mustard-200 md:w-3/4 md:border-x md:text-4xl">
                <button
                  type="button"
                  onClick={() => resetMutation()}
                  className="absolute right-2 top-2"
                >
                  <XMarkIcon className="w-7" />
                  <span className="sr-only">Close transferred key message</span>
                </button>
                <span>Transferred {response.data.keys.length} keys</span>
                <ul className="grid gap-x-16 text-base lg:grid-cols-2">
                  {response.data.keys.map((key) => (
                    <li key={key}>{key}</li>
                  ))}
                </ul>
              </div>
            )}
            {mutationStatus === 'error' && (
              <FormError>
                <span>
                  We had some trouble claiming this account. Give it another try
                  and if it fails again, create an issue in{' '}
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
            <div className="flex justify-center gap-6">
              <Button
                type={Button.Types.submit}
                appearance={Button.Appearances.solid}
                color={Button.Colors.primary}
                size={Button.Sizes.xl}
                disabled={mutationStatus === 'loading'}
                busy={mutationStatus === 'loading'}
              >
                claim account
              </Button>
              <RouterButtonLink
                to="/self-service/keys"
                appearance={Button.Appearances.outline}
                color={Button.Colors.secondary}
                size={Button.Sizes.xl}
              >
                manage keys
              </RouterButtonLink>
            </div>
          </form>
        </div>
      </section>
    </>
  );
}
Component.displayName = 'CreateKey';
