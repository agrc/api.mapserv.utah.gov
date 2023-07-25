import { ExclamationTriangleIcon } from '@heroicons/react/24/outline';
import PropTypes from 'prop-types';

export const FormErrors = ({ errors }) => {
  const entries = Object.entries(errors);

  if (entries.length === 0) {
    return null;
  }

  return (
    <div className="m-6 mx-auto flex min-h-[75px] max-w-lg flex-row gap-2 rounded border border-fuchsia-500 dark:bg-slate-700 dark:text-slate-100">
      <div className="inline-flex min-w-[75px] items-center justify-center bg-fuchsia-200 text-fuchsia-500/70">
        <ExclamationTriangleIcon className="h-10" />
      </div>
      <div className="px-3 py-2">
        <span className="inline-block font-bold">
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

export const FormError = ({ message }) => {
  if (message?.length === 0) {
    return null;
  }

  return (
    <div className="m-6 mx-auto flex min-h-[75px] max-w-lg flex-row gap-2 rounded border border-fuchsia-500 dark:bg-slate-700 dark:text-slate-100 ">
      <div className="inline-flex min-w-[75px] items-center justify-center bg-fuchsia-200 text-fuchsia-500/70">
        <ExclamationTriangleIcon className="h-10" />
      </div>
      <div className="self-center px-3 py-2 font-bold">{message}</div>
    </div>
  );
};
FormError.propTypes = {
  message: PropTypes.string,
};
