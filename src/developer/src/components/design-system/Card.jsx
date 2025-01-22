import { clsx } from 'clsx';
import PropTypes from 'prop-types';

const Card = ({ title, subTitle, children, danger = false }) => {
  return (
    <div
      className={clsx('mt-6 border shadow-md md:mt-0', {
        'border-slate-300 dark:border-slate-800': !danger,
        'border-red-900 dark:border-rose-700': danger,
        'bg-slate-100 dark:bg-slate-700': !danger,
        'bg-red-300 dark:bg-rose-950': danger,
      })}
    >
      {title && (
        <h4
          className={clsx('bg-white px-5 pt-3 uppercase dark:bg-slate-600', {
            'pb-3': !subTitle,
            'text-primary-600 dark:text-slate-50': !danger,
            'text-rose-700 dark:text-rose-200': danger,
          })}
        >
          {title}
        </h4>
      )}
      {subTitle && (
        <p className="bg-white px-5 pb-3 text-slate-600 dark:bg-slate-600 dark:text-slate-200">{subTitle}</p>
      )}
      <div className="border-t border-slate-200 pb-6 pt-3 dark:border-slate-800">{children}</div>
    </div>
  );
};
Card.propTypes = {
  title: PropTypes.string.isRequired,
  children: PropTypes.node.isRequired,
  subTitle: PropTypes.string,
  danger: PropTypes.bool,
};

export default Card;
