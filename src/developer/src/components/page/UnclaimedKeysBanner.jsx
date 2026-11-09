import { ExclamationTriangleIcon } from '@heroicons/react/20/solid';
import { Link } from 'react-router';

const UNCLAIMED_KEYS_CUTOFF_DATE = 'February 1, 2027';
const UTAHID_MIGRATION_DATE = 'August 5, 2024';

const UnclaimedKeysBanner = () => (
  <aside
    aria-label="Unclaimed API keys notice"
    className="border-b border-amber-300 bg-amber-50 px-4 py-3 text-amber-950 dark:border-amber-800 dark:bg-amber-950 dark:text-amber-50"
  >
    <div className="mx-auto flex max-w-5xl items-start gap-3 sm:items-center">
      <ExclamationTriangleIcon
        className="mt-0.5 size-5 shrink-0 text-amber-600 sm:mt-0 dark:text-amber-400"
        aria-hidden
      />
      <p className="m-0">
        <strong>Unclaimed API keys will be disabled on {UNCLAIMED_KEYS_CUTOFF_DATE}.</strong> Keys created before the
        UGRC API moved to UtahID sign in on {UTAHID_MIGRATION_DATE} must be claimed to keep working.{' '}
        <Link
          to="/self-service/claim-account"
          className="font-semibold whitespace-nowrap underline underline-offset-2 hover:decoration-2"
        >
          Claim your keys
        </Link>
      </p>
    </div>
  </aside>
);

export default UnclaimedKeysBanner;
