import PropTypes from 'prop-types';
import { Link } from 'react-router';
import { twMerge } from 'tailwind-merge';
import { createKeyLookup } from '.';
import Spinner from './Spinner';

const COLORS = {
  none: {
    outlined:
      'border-slate-600 text-slate-600 focus:border-slate-600 focus:ring-slate-600 hover:border-slate-600 hover:bg-slate-600',
    solid:
      'border-slate-600 bg-slate-600 text-white focus:border-slate-600 focus:ring-slate-600 hover:border-slate-700 hover:bg-slate-700',
  },
  primary: {
    outlined:
      'border-primary-500 text-primary-500 focus:border-primary-500 focus:ring-primary-500 hover:border-primary-500 hover:bg-primary-500',
    solid:
      'border-primary-600 bg-primary-500 text-secondary-100 focus:border-primary-700 focus:ring-primary-700 hover:border-primary-600 hover:bg-primary-700 border border-2 dark:border-secondary-600/50',
  },
  secondary: {
    outlined:
      'bg-white/75 dark:bg-transparent border-secondary-600 text-secondary-500 dark:text-secondary-100 focus:border-secondary-400 focus:ring-secondary-400 dark:hover:text-slate-600 dark:hover:bg-secondary-400 hover:text-slate-600 hover:bg-secondary-400',
    solid:
      'bg-secondary-500/75 border-secondary-600 text-white focus:border-secondary-400 focus:ring-secondary-400 hover:text-slate-600 hover:border-secondary-400 hover:bg-secondary-400',
  },
  accent: {
    outlined:
      'border-accent text-accent focus:border-accent focus:ring-accent hover:border-accent hover:bg-accent',
    solid:
      'border-accent bg-accent text-white focus:border-accent focus:ring-accent hover:border-accent hover:bg-accent',
  },
};

const APPEARANCES = {
  solid: 'solid',
  outlined: 'outlined',
};

const SIZES = {
  xs: 'text-xs px-2 py-0',
  sm: 'text-sm px-3 py-0 h-6',
  base: 'px-7 py-1 h-8 min-h-[2rem]',
  lg: 'text-lg px-8 py-2 h-10 min-h-[2.5rem]',
  xl: 'text-xl px-10 pt-2 pb-3 h-12 min-h-[3rem]',
};

const TYPES = {
  button: 'button',
  submit: 'submit',
  reset: 'reset',
};

function Button({
  appearance = 'outlined',
  busy = false,
  children,
  className,
  color = 'none',
  disabled = false,
  onClick,
  size = 'base',
  title,
  type = 'button',
}) {
  if (busy) {
    disabled = true;
  }

  return (
    <button
      className={twMerge(
        'flex w-fit cursor-pointer select-none items-center justify-center rounded-full',
        appearance === APPEARANCES.outlined && 'border-2',
        COLORS[color][appearance],
        SIZES[size],
        'transition-all duration-200 ease-in-out',
        'focus:outline-none focus:ring-2 focus:ring-opacity-50',
        'hover:text-white',
        !disabled && 'active:scale-95 active:shadow-inner',
        'disabled:cursor-not-allowed disabled:opacity-50',
        className,
      )}
      disabled={disabled}
      onClick={onClick}
      title={title}
      type={type}
    >
      {children}
      {busy && <Spinner className="ml-1" ariaLabel="loading" size={size} />}
    </button>
  );
}

Button.propTypes = {
  appearance: PropTypes.oneOf(Object.keys(APPEARANCES)),
  busy: PropTypes.bool,
  children: PropTypes.node.isRequired,
  className: PropTypes.string,
  color: PropTypes.oneOf(Object.keys(COLORS)),
  disabled: PropTypes.bool,
  onClick: PropTypes.func,
  title: PropTypes.string,
  type: PropTypes.oneOf(Object.keys(TYPES)),
  size: PropTypes.oneOf(Object.keys(SIZES)),
};

Button.Colors = createKeyLookup(COLORS);
Button.Appearances = createKeyLookup(APPEARANCES);
Button.Sizes = createKeyLookup(SIZES);
Button.Types = createKeyLookup(TYPES);

export const RouterButtonLink = ({
  to,
  children,
  className,
  appearance = 'outlined',
  color = 'none',
  size = 'base',
}) => {
  return (
    <Link
      to={to}
      className={twMerge(
        'flex w-fit cursor-pointer select-none items-center justify-center rounded-full',
        appearance === APPEARANCES.outlined && 'border-2',
        COLORS[color][appearance],
        SIZES[size],
        'transition-all duration-200 ease-in-out',
        'focus:outline-none focus:ring-2 focus:ring-opacity-50',
        className,
      )}
    >
      {children}
    </Link>
  );
};
RouterButtonLink.displayName = 'RouterButtonLink';
RouterButtonLink.propTypes = {
  appearance: PropTypes.oneOf(Object.keys(APPEARANCES)),
  children: PropTypes.node.isRequired,
  className: PropTypes.string,
  color: PropTypes.oneOf(Object.keys(COLORS)),
  size: PropTypes.oneOf(Object.keys(SIZES)),
  to: PropTypes.string.isRequired,
};

export default Button;
