import PropTypes from 'prop-types';
import { twMerge } from 'tailwind-merge';
import { createKeyLookup } from './';

const SIZES = {
  xs: 'h-3 w-3',
  sm: 'h-3.5 w-3.5',
  base: 'h-4 w-4',
  lg: 'h-[1.125rem] w-[1.125rem]',
  xl: 'h-5 w-5',
  custom: null,
};
export default function Spinner({ className, ariaLabel, size }) {
  return (
    <svg
      aria-live="polite"
      role="progressbar"
      aria-hidden="false"
      aria-valuetext={ariaLabel}
      className={twMerge(
        SIZES[size],
        'flex-shrink-0 animate-spin motion-reduce:hidden',
        className,
      )}
      xmlns="http://www.w3.org/2000/svg"
      fill="none"
      viewBox="0 0 24 24"
    >
      <circle
        className="opacity-25"
        cx="12"
        cy="12"
        r="10"
        stroke="currentColor"
        strokeWidth="4"
      ></circle>
      <path
        className="opacity-75"
        fill="currentColor"
        d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
      ></path>
    </svg>
  );
}

Spinner.propTypes = {
  className: PropTypes.string,
  ariaLabel: PropTypes.string.isRequired,
  /**
   * Size of the spinner. Corresponds with the tailwind text sizes (base, sm, lg, xl).
   * Use `custom` and pass your own height and width via `className` for custom sizes.
   */
  size: PropTypes.oneOf(Object.keys(SIZES)),
};

Spinner.Sizes = createKeyLookup(SIZES);

Spinner.defaultProps = {
  size: 'base',
};
