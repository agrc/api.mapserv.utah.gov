import clsx from 'clsx';
import md5 from 'md5';
import PropTypes from 'prop-types';
import Popover from './design-system/Popover';
import Tooltip from './design-system/Tooltip';

const size = 64;

const gravatarIcon = (
  <svg
    className="absolute bottom-1 right-1 h-3 w-3 fill-current text-slate-800/20 dark:text-slate-100/20"
    xmlns="http://www.w3.org/2000/svg"
    viewBox="0 0 27 27"
    role="presentation"
    aria-hidden="true"
  >
    <path d="M10.8 2.699v9.45a2.699 2.699 0 005.398 0V5.862a8.101 8.101 0 11-8.423 1.913 2.702 2.702 0 00-3.821-3.821A13.5 13.5 0 1013.499 0 2.699 2.699 0 0010.8 2.699z"></path>
  </svg>
);

const Avatar = ({ anonymous = true, user = {}, signOut }) => {
  if (anonymous || anonymous === undefined || anonymous === null) {
    return null;
  }

  return (
    <Popover
      trigger={
        <div className="flex w-full flex-col items-center gap-6 cursor-pointer">
          <span className="relative">
            <span className="mr-2 inline-block w-16 h-16 overflow-hidden rounded-full border-2 border-wavy-500 bg-wavy-500 shadow-lg">
              <Gravatar email={user.email} name={user.displayName} />
            </span>
            <Tooltip trigger={gravatarIcon} delayDuration={300}>
              Update your profile image on{' '}
              <a href="https://gravatar.com">Gravatar</a>.
            </Tooltip>
          </span>
        </div>
      }
    >
      <div className="grid grid-cols-1 divide-y whitespace-nowrap">
        <a
          href="https://id.utah.gov"
          className="text-slate-500 dark:text-slate-300 text-sm p-1 flex items-center justify-between hover:text-slate-600"
        >
          UtahID Profile
        </a>
        <button
          className="text-slate-500 dark:text-slate-300 text-sm p-1 flex items-center justify-between hover:text-slate-600"
          onClick={() => signOut()}
          type="button"
        >
          Sign out
        </button>
      </div>
    </Popover>
  );
};
Avatar.propTypes = {
  anonymous: PropTypes.bool,
  user: PropTypes.shape({
    email: PropTypes.string,
    displayName: PropTypes.string,
  }),
  signOut: PropTypes.func,
};
Avatar.displayName = 'Avatar';

const Gravatar = ({ email, name = '' }) => {
  const gravatar = `https://www.gravatar.com/avatar/${md5(
    email.toLowerCase(),
  )}?size=${size}&default=blank`;

  const initials = getInitials(name);

  return (
    <div className="relative w-full h-full inline-flex items-center justify-center">
      <div
        aria-hidden="true"
        className={clsx(
          'text-white dark:text-black opacity-60 text-center text-2xl font-extralight select-none',
          {
            'text-3xl': initials.length < 3,
            'text-2xl': initials.length === 3,
          },
        )}
      >
        {initials}
      </div>
      <img
        src={gravatar}
        className="absolute top-0 left-0"
        alt={`${name}'s Gravatar`}
      />
    </div>
  );
};
Gravatar.propTypes = {
  email: PropTypes.string.isRequired,
  name: PropTypes.string,
};

const getInitials = (name) => {
  name = name.trim();

  if (name.length <= 3) {
    return name;
  }

  return name
    .split(/\s+/)
    .map((w) => [...w][0])
    .slice(0, 3)
    .join('');
};

export default Avatar;
