import * as Label from '@radix-ui/react-label';
import PropTypes from 'prop-types';
import { forwardRef } from 'react';
import { twJoin, twMerge } from 'tailwind-merge';

const TextArea = forwardRef(
  (
    {
      className,
      disabled,
      error,
      id,
      inline,
      invalid,
      label,
      message,
      name,
      onBlur,
      onChange,
      placeholder,
      required,
      rows,
      value,
    },
    ref,
  ) => {
    if (!id) {
      id = label.toLowerCase().replace(' ', '-');
    }

    console.log('textarea', error, invalid);
    if (error) {
      invalid = true;
    }

    return (
      <div
        className={twMerge(
          inline ? 'inline-flex items-center' : 'flex flex-col',
          className,
        )}
      >
        <Label.Root asChild className={inline && 'mr-2'} htmlFor={id}>
          <strong>
            {label}
            {required && (
              <span className="ml-[0.1rem] text-fuchsia-500">*</span>
            )}
          </strong>
        </Label.Root>
        <div className="flex flex-1 flex-col">
          <div className="space-y-1">
            {error && (
              <p className="ml-2 text-sm dark:text-fuchsia-200 text-fuchsia-500">
                {error}
              </p>
            )}
            <textarea
              ref={ref}
              className={twMerge(
                'resize-none w-full rounded-md border-slate-400 px-2 py-1 text-slate-700 shadow-sm transition-all duration-200 ease-in-out placeholder:text-slate-400 focus:border-mustard-500 focus:outline-none focus:ring focus:ring-mustard-600 focus:ring-opacity-50 disabled:cursor-not-allowed disabled:opacity-50 sm:text-sm',
                !inline && 'w-full',
                invalid &&
                  'border-2 border-fuchsia-500 focus:border-fuchsia-500 focus:ring-fuchsia-600',
                disabled
                  ? 'cursor-not-allowed border-slate-300 bg-slate-100'
                  : 'bg-white',
              )}
              rows={rows}
              id={id}
              disabled={disabled}
              value={value}
              name={name}
              placeholder={placeholder}
              onChange={(event) => onChange(event.target.value)}
              onBlur={onBlur}
              required={required}
              aria-required={required}
              aria-labelledby={label ? `label.${name}` : null}
            />
          </div>
          {message && (
            <span className={twJoin('text-sm', invalid && 'text-fuchsia-500')}>
              {message}
            </span>
          )}
        </div>
      </div>
    );
  },
);
TextArea.displayName = 'TextArea';
TextArea.propTypes = {
  className: PropTypes.string,
  disabled: PropTypes.bool,
  error: PropTypes.string,
  id: PropTypes.string,
  inline: PropTypes.bool,
  invalid: PropTypes.bool,
  label: PropTypes.string,
  message: PropTypes.string,
  name: PropTypes.string,
  onBlur: PropTypes.func,
  onChange: PropTypes.func,
  placeholder: PropTypes.string,
  required: PropTypes.bool,
  rows: PropTypes.number,
  value: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
};
TextArea.defaultProps = {
  rows: 3,
};

export default TextArea;
