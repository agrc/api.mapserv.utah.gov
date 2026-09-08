import { useFirebaseAuth } from '@ugrc/utah-design-system/contexts/FirebaseAuthProvider';
import { setUtahFooterSettings, setUtahHeaderSettings } from '@utahdts/utah-design-system-header';
import { useEffect } from 'react';

const links = [
  {
    actionUrl: {
      url: 'https://www.utah.gov/support/disclaimer.html',
      openInNewTab: true,
    },
    title: 'Terms of use',
  },
  {
    actionUrl: {
      url: 'https://www.utah.gov/support/privacypolicy.html',
      openInNewTab: true,
    },
    title: 'Privacy policy',
  },
  {
    actionUrl: {
      url: 'https://www.utah.gov/support/accessibility.html',
      openInNewTab: true,
    },
    title: 'Accessibility',
  },
  {
    actionUrl: {
      url: 'https://www.utah.gov/support/translate.html',
      openInNewTab: true,
    },
    title: 'Translate',
  },
  {
    actionUrl: { url: '/ThirdPartyNotices.txt' },
    title: 'Third-party notices',
  },
];

const getUserInfo = (currentUser) => {
  const nameParts = currentUser?.displayName?.split(' ') ?? [];

  return {
    authenticated: Boolean(currentUser),
    first: nameParts[0] ?? null,
    last: nameParts.slice(1).join(' ') || null,
    id: currentUser?.uid ?? null,
    mail: currentUser?.email ? [currentUser.email] : null,
    username: currentUser?.email ?? null,
  };
};

const UtahChrome = () => {
  const { currentUser, login, logout } = useFirebaseAuth();

  useEffect(() => {
    setUtahHeaderSettings({
      applicationType: 'custom application',
      title: 'UGRC API',
      titleUrl: '/',
      skipLinkUrl: '#main-content',
      showTitle: true,
      size: 'MEDIUM',
      mainMenu: false,
      utahId: {
        currentUser: getUserInfo(currentUser),
        onSignIn: () => login(),
        onSignOut: () => logout(),
        notifications: false,
      },
      onSearch: false,
      logo: { imageUrl: '/logo.svg' },
      domLocationTarget: { cssSelector: '#utah-header' },
      actionItems: [
        {
          icon: '<span class="utds-icon-before-waffle" aria-hidden="true" />',
          showTitle: false,
          title: 'More links',
          actionPopupMenu: {
            title: 'More links',
            menuItems: links,
          },
        },
      ],
    });

    setUtahFooterSettings({
      domLocationTarget: { cssSelector: '#utah-footer' },
      linkPrivacyPolicy: 'https://www.utah.gov/support/privacypolicy.html',
      linkTermsOfUse: 'https://www.utah.gov/support/disclaimer.html',
      showHorizontalRule: false,
    });
  }, [currentUser, login, logout]);

  return <div id="utah-header" />;
};

export default UtahChrome;
