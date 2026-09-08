import { FirebaseAnalyticsProvider } from '@ugrc/utah-design-system/contexts/FirebaseAnalyticsProvider';
import { FirebaseAuthProvider } from '@ugrc/utah-design-system/contexts/FirebaseAuthProvider';
import { FirebaseFunctionsProvider } from '@ugrc/utah-design-system/contexts/FirebaseFunctionsProvider';
import { FirestoreProvider } from '@ugrc/utah-design-system/contexts/FirestoreProvider';
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
