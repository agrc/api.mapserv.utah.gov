import PropTypes from 'prop-types';

const Card = ({ title, subTitle, children }) => {
  return (
    <div className="mt-6 border border-slate-300 bg-slate-100 shadow-md dark:border-slate-800 dark:bg-slate-700 md:mt-0">
      {title && (
        <h4 className="bg-white px-5 pt-3 uppercase text-wavy-600 dark:bg-slate-600 dark:text-slate-50">
          {title}
        </h4>
      )}
      {subTitle && (
        <p className="bg-white px-5 pb-3 text-slate-600 dark:bg-slate-600 dark:text-slate-200">
          {subTitle}
        </p>
      )}
      <div className="border-t border-slate-200 pb-6 pt-3 dark:border-slate-800">
        {children}
      </div>
    </div>
  );
};
Card.propTypes = {
  title: PropTypes.string.isRequired,
  children: PropTypes.node.isRequired,
  subTitle: PropTypes.string,
};

export default Card;
