import { getAuth } from 'firebase/auth';
import { getFirestore } from 'firebase/firestore';
import { getFunctions } from 'firebase/functions';
import PropTypes from 'prop-types';
import {
  AuthProvider,
  FirestoreProvider,
  FunctionsProvider,
  useFirebaseApp,
} from 'reactfire';

const FirebaseContainer = ({ children }) => {
  const app = useFirebaseApp();
  const auth = getAuth(app);
  const functions = getFunctions(app);
  const firestore = getFirestore(app);

  return (
    <AuthProvider sdk={auth}>
      <FirestoreProvider sdk={firestore}>
        <FunctionsProvider sdk={functions}>{children}</FunctionsProvider>
      </FirestoreProvider>
    </AuthProvider>
  );
};
FirebaseContainer.propTypes = {
  children: PropTypes.node,
};

export default FirebaseContainer;
