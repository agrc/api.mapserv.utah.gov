import PropTypes from 'prop-types';

export const TextLink = ({ href, target, children }) => {
  return (
    <a
      href={href}
      target={target}
      className="font-medium italic text-mustard-900 underline decoration-mustard-500 underline-offset-2 transition-all hover:decoration-2 hover:underline-offset-4 focus:outline-none active:text-mustard-500 active:decoration-mustard-600 active:underline-offset-8 dark:text-mustard-400/70"
      rel={target === '_blank' ? 'noopener noreferrer' : undefined}
    >
      {children}
    </a>
  );
};
TextLink.displayName = 'TextLink';
TextLink.propTypes = {
  href: PropTypes.string.isRequired,
  target: PropTypes.oneOf(['_blank', '_self', '_parent', '_top']),
  children: PropTypes.node.isRequired,
};
TextLink.defaultProps = {
  target: '_self',
};
TextLink.target = ['_blank', '_self', '_parent', '_top'];
