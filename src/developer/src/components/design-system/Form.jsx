import PropTypes from 'prop-types';
import { Banner } from './Banner';

export const FormErrors = ({ errors }) => {
  const entries = Object.entries(errors);

  if (entries.length === 0) {
    return null;
  }

  return (
    <Banner>
      <div className="px-3 py-2">
        <span className="inline-block font-bold">
          Some errors have been found:
        </span>
        <ul className="list-inside">
          {entries.map(([key, value]) => {
            return (
              <li className="ml-2 list-disc text-sm" key={key}>
                {value.message}
              </li>
            );
          })}
        </ul>
      </div>
    </Banner>
  );
};
FormErrors.propTypes = {
  errors: PropTypes.object.isRequired,
};

export const FormError = ({ children }) => {
  if (children?.length === 0) {
    return null;
  }

  return (
    <Banner>
      <div className="self-center px-3 py-2 font-bold">{children}</div>
    </Banner>
  );
};
FormError.propTypes = {
  children: PropTypes.node,
};
