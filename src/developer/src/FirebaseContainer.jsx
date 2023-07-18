import { getAuth } from 'firebase/auth';
import { getFunctions } from 'firebase/functions';
import PropTypes from 'prop-types';
import { AuthProvider, FunctionsProvider, useFirebaseApp } from 'reactfire';

const FirebaseContainer = ({ children }) => {
  const app = useFirebaseApp();
  const auth = getAuth(app);
  const functions = getFunctions(app);

  return (
    <AuthProvider sdk={auth}>
      <FunctionsProvider sdk={functions}>{children}</FunctionsProvider>
    </AuthProvider>
  );
};
FirebaseContainer.propTypes = {
  children: PropTypes.node,
};

export default FirebaseContainer;
