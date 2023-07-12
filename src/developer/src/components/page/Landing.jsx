import UtahIdLogin from '../UtahIdLogin';
import product from './product.png';

const LandingPage = () => (
  <section className="max-w-full mx-auto px-3 lg:px-0">
    <div className="flex flex-wrap">
      <div className="w-full max-w-full min-h-[480px] lg:min-h-[980px] py-10 lg:flex-basis-auto lg:w-1/2 lg:grow-0 lg:shrink-0 lg:py-[114px]">
        <div className="mx-auto w-full max-w-[480px] grid gap-6">
          <h2 className="text-wavy-800 dark:text-slate-100">
            Access your keys
          </h2>
          <p className="text-slate-700 dark:text-slate-300">
            The UGRC API requires a UtahID account to create and manage API
            keys. Your name and email address will be shared with this
            application.
          </p>
          <p className="text-slate-700 dark:text-slate-300">
            You will be able to claim your existing API keys by linking your
            prior account to your UtahId account after signing in.
          </p>
          <p className="text-slate-700 dark:text-slate-300">
            API keys are anonymous and randomly generated. No personal
            information will be shared or made public.{' '}
            <a href="/">View our privacy policy</a>.
          </p>
          <div className="flex items-center text-slate-500 mx-3 my-3">
            <span className="h-px flex-1 bg-slate-200"></span>
            <span className="mx-3 text-xs uppercase tracking-wide">
              continue with
            </span>
            <span className="h-px flex-1 bg-slate-200"></span>
          </div>
          <div className="flex justify-center">
            <UtahIdLogin />
          </div>
        </div>
      </div>
      <div className="relative hidden flex-col items-center justify-center bg-gradient-to-b to-mustard-400 via-wavy-400 from-wavy-700 lg:flex-basis-auto lg:w-1/2 lg:grow-0 lg:shrink-0 lg:block">
        <img
          className="absolute left-0 top-0 h-full w-full"
          src="/sparkles.png"
          alt=""
        ></img>
        <h2 className="text-white text-center w-full mt-12">
          Location Matters
        </h2>
        <h3 className="text-white text-center w-full">
          Let data take you places
        </h3>
        <div>
          <img
            width="667"
            height="557"
            className="mx-auto grayscale hover:grayscale-0"
            src={product}
            alt=""
          ></img>
        </div>
      </div>
    </div>
  </section>
);

export default LandingPage;
