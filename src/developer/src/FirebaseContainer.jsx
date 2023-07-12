import { getAuth } from 'firebase/auth';
import PropTypes from 'prop-types';
import { AuthProvider, useFirebaseApp } from 'reactfire';

const FirebaseContainer = ({ children }) => {
  const app = useFirebaseApp();
  const auth = getAuth(app);

  return <AuthProvider sdk={auth}>{children}</AuthProvider>;
};
FirebaseContainer.propTypes = {
  children: PropTypes.node,
};

export default FirebaseContainer;
