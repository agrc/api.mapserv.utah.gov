import { Banner } from '@ugrc/utah-design-system/components/Banner';
import { ExternalLink } from '@ugrc/utah-design-system/components/Link';
import { UtahIdLogin } from '@ugrc/utah-design-system/components/UtahIdLogin';
import product from './product.png';

export function Component() {
  return (
    <section className="mx-auto max-w-full px-3 lg:px-0">
      <div className="flex flex-wrap">
        <div className="lg:flex-basis-auto min-h-120 w-full max-w-full py-10 lg:min-h-245 lg:w-1/2 lg:shrink-0 lg:grow-0 lg:py-28.5">
          <div className="mx-auto grid w-full max-w-120 gap-6">
            <h2 className="text-primary-800 dark:text-slate-100">Access your keys</h2>
            <p className="text-primary-900 dark:text-slate-100">
              The UGRC API requires a UtahID account to create and manage API keys. Your name and email address will be
              shared with this application.
            </p>
            <p className="text-primary-900 dark:text-slate-100">
              You will be able to claim your existing API keys by linking your prior account to your UtahId account
              after signing in.
            </p>
            <p className="text-primary-900 dark:text-slate-100">
              API keys are anonymous and randomly generated. No personal information will be shared or made public.{' '}
              <ExternalLink href={`${import.meta.env.VITE_API_EXPLORER_URL}/privacy/`}>
                View our privacy policy
              </ExternalLink>
              .
            </p>
            <div className="utah-id-login">
              <UtahIdLogin
                size="extraLarge"
                errorRenderer={(error) => {
                  return (
                    <Banner>
                      <div className="grid gap-4">{error}</div>
                    </Banner>
                  );
                }}
              />
            </div>
          </div>
        </div>
        <div className="lg:flex-basis-auto from-primary-700 via-primary-400 to-secondary-400 relative hidden flex-col items-center justify-center bg-gradient-to-b lg:block lg:w-1/2 lg:shrink-0 lg:grow-0">
          <img className="absolute top-0 left-0 h-full w-full" src="/sparkles.png" loading="lazy" alt=""></img>
          <h2 className="mt-12 w-full text-center text-white">Location Matters</h2>
          <h3 className="w-full text-center text-white">Let data take you places</h3>
          <div>
            <img
              width="667"
              height="557"
              className="mx-auto grayscale hover:grayscale-0"
              src={product}
              loading="lazy"
              alt=""
            ></img>
          </div>
        </div>
      </div>
    </section>
  );
}
Component.displayName = 'Unauthenticated';
