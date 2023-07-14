import * as Label from '@radix-ui/react-label';
import PropTypes from 'prop-types';
import { forwardRef } from 'react';
import { twJoin, twMerge } from 'tailwind-merge';

const Input = forwardRef(
  (
    {
      className,
      disabled,
      error,
      id,
      inline,
      invalid,
      label,
      max,
      message,
      min,
      name,
      onChange,
      placeholder,
      required,
      step,
      type = 'text',
      value,
    },
    ref,
  ) => {
    if (!id) {
      id = label.toLowerCase().replace(' ', '-');
    }

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
            {error && <p className="ml-2 text-sm text-fuchsia-200">{error}</p>}
            <input
              ref={ref}
              className={twMerge(
                'h-10 w-full rounded-md border-slate-400 px-2 py-1 text-slate-700 shadow-sm transition-all duration-200 ease-in-out placeholder:text-slate-400 focus:border-mustard-500 focus:outline-none focus:ring focus:ring-mustard-600 focus:ring-opacity-50 disabled:cursor-not-allowed disabled:opacity-50 sm:text-sm',
                !inline && 'w-full',
                invalid &&
                  'border-2 border-fuchsia-500 focus:border-fuchsia-500 focus:ring-fuchsia-600',
                disabled
                  ? 'cursor-not-allowed border-slate-300 bg-slate-100'
                  : 'bg-white',
              )}
              type={type}
              min={min}
              max={max}
              step={step}
              id={id}
              disabled={disabled}
              defaultValue={value}
              name={name}
              placeholder={placeholder}
              onChange={(event) => onChange(event.target.value)}
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
Input.displayName = 'Input';
Input.propTypes = {
  className: PropTypes.string,
  disabled: PropTypes.bool,
  error: PropTypes.string,
  id: PropTypes.string,
  inline: PropTypes.bool,
  invalid: PropTypes.bool,
  label: PropTypes.string.isRequired,
  max: PropTypes.number,
  message: PropTypes.string,
  min: PropTypes.number,
  name: PropTypes.string,
  onChange: PropTypes.func,
  placeholder: PropTypes.string,
  required: PropTypes.bool,
  step: PropTypes.number,
  type: PropTypes.oneOf([
    'date',
    'datetime-local',
    'email',
    'hidden',
    'month',
    'number',
    'password',
    'search',
    'tel',
    'text',
    'time',
    'url',
    'week',
  ]),
  value: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
};

export default Input;
