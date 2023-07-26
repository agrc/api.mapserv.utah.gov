import { ChevronDownIcon } from '@heroicons/react/20/solid';
import * as NavigationMenu from '@radix-ui/react-navigation-menu';
import clsx from 'clsx';
import PropTypes from 'prop-types';
import { forwardRef } from 'react';
import { Link, useLocation } from 'react-router-dom';

const menuTextCss = (isActive) =>
  clsx(
    'group relative flex select-none items-center justify-between gap-1 px-3 py-2 font-bold leading-none text-wavy-900 outline-none dark:text-wavy-100',
    {
      'before:absolute before:-top-1.5 before:left-0 before:z-10 before:block before:h-1 before:w-full before:rounded-full before:bg-mustard-500':
        isActive,
    }
  );

const menuItemCss =
  'absolute left-0 top-0 w-full data-[motion=from-end]:animate-in data-[motion=from-end]:slide-in-from-right data-[motion=from-start]:animate-in data-[motion=from-start]:slide-in-from-left data-[motion=to-end]:animate-out data-[motion=to-end]:slide-out-to-right data-[motion=to-start]:animate-out data-[motion=to-start]:slide-out-to-left sm:w-auto';

const Menu = () => {
  return (
    <NavigationMenu.Root className="relative z-10 flex w-screen justify-start border-b border-dashed border-b-wavy-300 bg-wavy-200/50">
      <NavigationMenu.List className=" flex list-none justify-center p-1">
        <NavigationMenu.Item className="rounded-full hover:bg-wavy-400/50">
          <MenuLink to="/self-service">Home</MenuLink>
        </NavigationMenu.Item>
        <NavigationMenu.Item className="rounded-full hover:bg-wavy-400/50">
          <MenuTrigger
            toArray={['/self-service/create-key', '/self-service/keys']}
          >
            Keys
          </MenuTrigger>
          <NavigationMenu.Content className={menuItemCss}>
            <ul className="one m-0 grid list-none p-5 sm:w-[300px]">
              <ListItem href="/self-service/create-key" title="Create Keys">
                Generate a new API Key.
              </ListItem>
              <ListItem href="/self-service/keys" title="Manage Keys">
                Pause, resume, or delete your API Keys and access analytics for
                the key.
              </ListItem>
            </ul>
          </NavigationMenu.Content>
        </NavigationMenu.Item>
        <NavigationMenu.Item className=" rounded-full hover:bg-wavy-400/50">
          <MenuTrigger toArray={[]}>Help</MenuTrigger>
          <NavigationMenu.Content className={menuItemCss}>
            <ul className="one m-0 grid list-none p-5 sm:w-[300px]">
              <ListItem
                href="https://ut-dts-agrc-web-api-dev.web.app/"
                title="API Home page"
              >
                Visit the API home page.
              </ListItem>
              <ListItem
                href="https://ut-dts-agrc-web-api-dev.web.app/en/documentation"
                title="API Documentation"
              >
                View the API documentation.
              </ListItem>
            </ul>
          </NavigationMenu.Content>
        </NavigationMenu.Item>
        <NavigationMenu.Indicator className="top-full z-10 flex h-1 items-end justify-center overflow-hidden bg-mustard-400 transition-all data-[state=visible]:animate-in data-[state=hidden]:animate-out data-[state=hidden]:fade-out data-[state=visible]:fade-in" />
      </NavigationMenu.List>

      <div className="absolute left-2 top-full flex w-full justify-start">
        <NavigationMenu.Viewport className="relative mt-2 h-[var(--radix-navigation-menu-viewport-height)] w-full overflow-hidden rounded border bg-white shadow data-[state=open]:animate-in data-[state=closed]:animate-out data-[state=closed]:zoom-out data-[state=open]:zoom-in dark:border-mustard-500/30 dark:bg-slate-900 sm:w-[var(--radix-navigation-menu-viewport-width)]" />
      </div>
    </NavigationMenu.Root>
  );
};
const MenuLink = ({ to, children }) => {
  const location = useLocation();
  const isActive = location.pathname === to;

  return (
    <NavigationMenu.Link asChild className={menuTextCss(isActive)}>
      <Link to={to}>{children}</Link>
    </NavigationMenu.Link>
  );
};
MenuLink.propTypes = {
  to: PropTypes.string.isRequired,
  children: PropTypes.node.isRequired,
};
const MenuTrigger = ({ toArray, children }) => {
  const location = useLocation();
  const isActive = toArray.includes(location.pathname);

  return (
    <NavigationMenu.Trigger className={menuTextCss(isActive)}>
      {children}
      <ChevronDownIcon
        className="relative h-3 transition-transform duration-300 ease-in group-data-[state=open]:-rotate-180"
        aria-hidden
      />
    </NavigationMenu.Trigger>
  );
};
MenuTrigger.propTypes = {
  toArray: PropTypes.arrayOf(PropTypes.string).isRequired,
  children: PropTypes.node.isRequired,
};

const ListItem = forwardRef(
  ({ className, children, title, ...props }, forwardedRef) => (
    <li className="leading-[.5em]">
      <NavigationMenu.Link asChild>
        <a
          className={clsx(
            'block select-none rounded p-3 text-base no-underline outline-none transition-colors hover:bg-slate-100 focus:shadow-[0_0_0_2px] focus:shadow-mustard-400 dark:hover:bg-slate-700/50',
            className
          )}
          {...props}
          ref={forwardedRef}
        >
          <div className="font-medium text-wavy-700 dark:text-mustard-600">
            {title}
          </div>
          <p className="text-sm text-wavy-500 dark:text-mustard-100/50">
            {children}
          </p>
        </a>
      </NavigationMenu.Link>
    </li>
  )
);
ListItem.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node.isRequired,
  title: PropTypes.string.isRequired,
};
ListItem.displayName = 'ListItem';

export default Menu;
