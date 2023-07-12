import PropTypes from 'prop-types';

const Pill = ({ children }) => {
  return (
    <div className="mx-1 my-1 rounded-full border border-slate-400 bg-white px-3 py-1 font-semibold tracking-wide dark:border-slate-800 dark:bg-slate-500 dark:text-slate-50">
      {children}
    </div>
  );
};
Pill.displayName = 'Pill';
Pill.propTypes = {
  children: PropTypes.node.isRequired,
};

export default Pill;
