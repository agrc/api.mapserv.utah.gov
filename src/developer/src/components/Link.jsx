import PropTypes from 'prop-types';
import { twMerge } from 'tailwind-merge';
import { createKeyLookup } from './design-system';

const COLORS = {
  primary:
    'decoration-mustard-500 active:text-mustard-500 active:decoration-mustard-600 dark:text-mustard-400/70',
  secondary:
    'decoration-mustard-500 dark:active:text-mustard-500 active:decoration-mustard-600 text-mustard-400/70',
};

export const TextLink = ({
  href,
  target = '_self',
  children,
  color = 'primary',
}) => {
  return (
    <a
      href={href}
      target={target}
      className={twMerge(
        'font-medium italic text-mustard-900 underline underline-offset-2 transition-all hover:decoration-2 hover:underline-offset-4 focus:outline-none active:underline-offset-8',
        COLORS[color],
      )}
      rel={target === '_blank' ? 'noopener noreferrer' : undefined}
    >
      {children}
    </a>
  );
};
TextLink.displayName = 'TextLink';
TextLink.propTypes = {
  href: PropTypes.string.isRequired,
  color: PropTypes.oneOf(Object.keys(COLORS)),
  target: PropTypes.oneOf(['_blank', '_self', '_parent', '_top']),
  children: PropTypes.node.isRequired,
};
TextLink.target = ['_blank', '_self', '_parent', '_top'];
TextLink.Colors = createKeyLookup(COLORS);
