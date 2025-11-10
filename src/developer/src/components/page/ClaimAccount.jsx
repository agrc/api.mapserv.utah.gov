import { XMarkIcon } from '@heroicons/react/20/solid';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Button,
  ExternalLink,
  FormError,
  FormErrors,
  Spinner,
  TextField,
  useFirebaseFunctions,
} from '@ugrc/utah-design-system';
import { httpsCallable } from 'firebase/functions';
import { Controller, useForm } from 'react-hook-form';
import { useNavigate } from 'react-router';
import * as z from 'zod';

const schema = z
  .object({
    email: z.email().nonempty(),
    password: z.string().nonempty(),
    confirm: z.string().nonempty(),
  })
  .refine((data) => data.password === data.confirm, {
    message: "Passwords don't match",
    path: ['confirm'], // path of error
  });

const defaultValues = {
  email: '',
  password: '',
  confirm: '',
};

export function Component() {
  const navigate = useNavigate();
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

  const { functions } = useFirebaseFunctions();
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
        <h2 id="key-claiming" className="mx-auto mb-4 max-w-5xl text-primary-800 dark:text-slate-100 md:col-span-2">
          Non-Utahid account key claiming
        </h2>
        <div className="mx-auto grid max-w-5xl grid-cols-1 gap-4 md:grid-cols-2 md:gap-10 md:px-6">
          <p className="text-primary-900 dark:text-slate-100">
            The UGRC API has migrated login systems. Due to this change, you currently do not have access to the keys
            created by your non-Utahid account. However, all of these keys will continue to function for the time being.
            To manage these keys and ensure their longevity, you will need to claim them from the account that
            originally created them.
          </p>
          <p className="text-primary-900 dark:text-slate-100">
            In order to do this, you will need to know the email address and password of the non-Utahid account that
            created the keys. The non-Utahid email address may even be the same as the Utahid account! And you can claim
            keys from multiple non-Utahid accounts if you have credentials for them.
          </p>
        </div>
      </section>
      <section className="relative w-full">
        <div className="bg-circuit absolute inset-0 h-64 bg-primary-600 shadow-lg"></div>
        <div className="relative z-10 mx-auto max-w-5xl px-6">
          <h3 className="mb-3 ml-2 pt-3 text-center text-white md:col-span-2">Claim keys from non-Utahid account</h3>
          <form
            onSubmit={handleSubmit(onSubmit)}
            className="mb-12 grid w-full gap-6 border border-slate-300 bg-slate-100 p-8 shadow-md dark:border-zinc-950 dark:bg-zinc-800"
          >
            <FormErrors errors={errors} />
            <div className="grid grid-cols-1 items-start gap-8 dark:text-slate-100">
              <Controller
                name="email"
                control={control}
                render={({ field }) => (
                  <TextField
                    label="Non-Utahid email"
                    placeholder="you@email.com"
                    description="This is the email address of the non-Utahid account that created the keys."
                    type="email"
                    isRequired
                    {...field}
                  />
                )}
              />
              <Controller
                name="password"
                control={control}
                render={({ field }) => <TextField label="Non-Utahid password" type="password" isRequired {...field} />}
              />
              <Controller
                name="confirm"
                control={control}
                render={({ field }) => <TextField label="Confirm password" type="password" isRequired {...field} />}
              />
            </div>

            {mutationStatus === 'pending' && (
              <div className="relative mx-auto mb-12 flex w-full items-center justify-center gap-6 border border-x-0 py-4 text-2xl font-black shadow dark:bg-slate-500 dark:text-secondary-200 md:w-3/4 md:border-x md:text-4xl">
                Claiming keys from account...
              </div>
            )}
            {mutationStatus === 'success' && (
              <>
                <div className="relative mx-auto flex w-full flex-col items-center justify-center gap-2 border border-x-0 border-primary-400/70 bg-slate-300/70 px-6 py-4 text-2xl font-black uppercase text-primary-500 shadow dark:bg-slate-500 dark:text-secondary-200 md:w-3/4 md:border-x md:text-4xl">
                  <button type="button" onClick={() => resetMutation()} className="absolute right-2 top-2">
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
                {response.data.keys.length === 0 && (
                  <div className="mx-auto rounded border border-red-900 bg-red-300 px-2 py-3 text-center text-rose-700 dark:border-rose-700 dark:bg-rose-950 dark:text-rose-200">
                    If this number does not look correct, your username and password values are incorrect.
                  </div>
                )}
              </>
            )}
            {mutationStatus === 'error' && (
              <FormError>
                <span>
                  We had some trouble claiming this account. Give it another try and if it fails again, create an issue
                  in <ExternalLink href="https://github.com/agrc/api.mapserv.utah.gov/issues/new">GitHub</ExternalLink>{' '}
                  or tweet us <ExternalLink href="https://x.com/maputah">@MapUtah</ExternalLink>.
                </span>
              </FormError>
            )}
            <div className="flex justify-center gap-6">
              <Button type="submit" size="extraLarge" isPending={mutationStatus === 'pending'}>
                claim account
              </Button>
              <Button onPress={() => navigate('/self-service/keys')} variant="secondary" size="extraLarge">
                manage keys
              </Button>
            </div>
          </form>
        </div>
      </section>
    </>
  );
}
Component.displayName = 'CreateKey';
