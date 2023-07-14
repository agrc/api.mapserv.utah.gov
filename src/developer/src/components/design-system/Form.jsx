import { ExclamationTriangleIcon } from '@heroicons/react/24/outline';
import PropTypes from 'prop-types';

export const FormErrors = ({ errors }) => {
  const entries = Object.entries(errors);

  if (entries.length === 0) {
    return null;
  }

  return (
    <div className="flex flex-row gap-2 border border-fuchsia-500 rounded min-h-[75px] dark:bg-slate-700 dark:text-slate-100 m-6 max-w-lg mx-auto">
      <div className="inline-flex justify-center items-center bg-fuchsia-200 text-fuchsia-500/70 min-w-[75px]">
        <ExclamationTriangleIcon className="h-10" />
      </div>
      <div className="px-3 py-2">
        <span className="font-bold inline-block">
          Some errors have been found:
        </span>
        <ul className="list-inside">
          {entries.map(([key, value]) => {
            return (
              <li className="ml-2 list-disc text-sm" key={key}>
                {value.message}
              </li>
            );
          })}
        </ul>
      </div>
    </div>
  );
};
FormErrors.propTypes = {
  errors: PropTypes.object.isRequired,
};
