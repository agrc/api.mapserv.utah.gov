import { getAnalytics } from 'firebase/analytics';
import { getAuth } from 'firebase/auth';
import { getFirestore } from 'firebase/firestore';
import { getFunctions } from 'firebase/functions';
import PropTypes from 'prop-types';
import {
  AnalyticsProvider,
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
  const analytics = getAnalytics(app);

  return (
    <AuthProvider sdk={auth}>
      <AnalyticsProvider sdk={analytics}>
        <FirestoreProvider sdk={firestore}>
          <FunctionsProvider sdk={functions}>{children}</FunctionsProvider>
        </FirestoreProvider>
      </AnalyticsProvider>
    </AuthProvider>
  );
};
FirebaseContainer.propTypes = {
  children: PropTypes.node,
};

export default FirebaseContainer;
