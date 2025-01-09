import {
  FirebaseAnalyticsProvider,
  FirebaseAuthProvider,
  FirebaseFunctionsProvider,
  FirestoreProvider,
} from '@ugrc/utah-design-system';
import { OAuthProvider } from 'firebase/auth';
import PropTypes from 'prop-types';

const provider = new OAuthProvider('oidc.utah-id');
provider.addScope('profile');
provider.addScope('email');

const FirebaseContainer = ({ children }) => {
  return (
    <FirebaseAuthProvider provider={provider}>
      <FirebaseAnalyticsProvider>
        <FirestoreProvider>
          <FirebaseFunctionsProvider>{children}</FirebaseFunctionsProvider>
        </FirestoreProvider>
      </FirebaseAnalyticsProvider>
    </FirebaseAuthProvider>
  );
};
FirebaseContainer.propTypes = {
  children: PropTypes.node,
};

export default FirebaseContainer;
