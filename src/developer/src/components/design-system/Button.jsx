import PropTypes from 'prop-types';
import { Link } from 'react-router-dom';
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
      'border-wavy-500 text-wavy-500 focus:border-wavy-500 focus:ring-wavy-500 hover:border-wavy-500 hover:bg-wavy-500',
    solid:
      'border-wavy-600 bg-wavy-500 text-mustard-100 focus:border-wavy-700 focus:ring-wavy-700 hover:border-wavy-600 hover:bg-wavy-700 border border-2 dark:border-mustard-600/50',
  },
  secondary: {
    outlined:
      'bg-white/75 dark:bg-transparent border-mustard-600 text-mustard-500 dark:text-mustard-100 focus:border-mustard-400 focus:ring-mustard-400 dark:hover:text-slate-600 dark:hover:bg-mustard-400 hover:text-slate-600 hover:bg-mustard-400',
    solid:
      'bg-mustard-500/75 border-mustard-600 text-white focus:border-mustard-400 focus:ring-mustard-400 hover:text-slate-600 hover:border-mustard-400 hover:bg-mustard-400',
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
  appearance,
  busy,
  children,
  className,
  color,
  disabled,
  onClick,
  size,
  title,
  type,
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

Button.defaultProps = {
  appearance: 'outlined',
  busy: false,
  color: 'none',
  disabled: false,
  type: 'button',
  size: 'base',
};

Button.Colors = createKeyLookup(COLORS);
Button.Appearances = createKeyLookup(APPEARANCES);
Button.Sizes = createKeyLookup(SIZES);
Button.Types = createKeyLookup(TYPES);

export const RouterButtonLink = ({
  to,
  children,
  className,
  appearance,
  color,
  size,
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
RouterButtonLink.defaultProps = {
  appearance: 'outlined',
  busy: false,
  color: 'none',
  disabled: false,
  size: 'base',
};

export default Button;
