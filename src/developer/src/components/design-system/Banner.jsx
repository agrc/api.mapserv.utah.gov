import { ExclamationTriangleIcon } from '@heroicons/react/24/outline';

import PropTypes from 'prop-types';

export const Banner = ({ children }) => {
  return (
    <div className="m-6 mx-auto flex min-h-[75px] max-w-lg flex-row gap-2 rounded border border-fuchsia-500 dark:bg-slate-700 dark:text-slate-100">
      <div className="inline-flex min-w-[75px] items-center justify-center rounded-l bg-fuchsia-200 text-fuchsia-500/70">
        <ExclamationTriangleIcon className="h-10" />
      </div>
      <div className="self-center px-3 py-2 font-bold">{children}</div>
    </div>
  );
};
Banner.propTypes = {
  children: PropTypes.node,
};
