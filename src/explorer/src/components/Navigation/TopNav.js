import React from 'react';

export default function TopNav() {
  return (
    <nav className="md:hidden relative fixed w-full bg-gray-800">
      <div className="max-w-7xl mx-auto px-2 sm:px-6 lg:px-8">
        <div className="relative flex items-center justify-between h-16">
          <div className="flex-1 flex items-center justify-center sm:items-stretch sm:justify-start">
            <div className="flex-shrink-0">
              <img className="block lg:hidden h-8 w-auto" src="/img/logos/workflow-mark-on-dark.svg" alt="" />
              <img className="hidden lg:block h-8 w-auto" src="/img/logos/workflow-logo-on-dark.svg" alt="" />
            </div>
            <div className="sm:ml-6">
              <div className="flex">
                <a
                  href="signup"
                  className="px-3 py-2 rounded-md text-sm font-medium leading-5 text-white bg-gray-900 focus:outline-none focus:text-white focus:bg-gray-700 transition duration-150 ease-in-out">
                  Sign up
                </a>
                <a
                  href="#geocoding"
                  className="ml-4 px-3 py-2 rounded-md text-sm font-medium leading-5 text-gray-300 hover:text-white hover:bg-gray-700 focus:outline-none focus:text-white focus:bg-gray-700 transition duration-150 ease-in-out">
                  Geocoding
                </a>
                <a
                  href="#searching"
                  className="ml-4 px-3 py-2 rounded-md text-sm font-medium leading-5 text-gray-300 hover:text-white hover:bg-gray-700 focus:outline-none focus:text-white focus:bg-gray-700 transition duration-150 ease-in-out">
                  Searching
                </a>
              </div>
            </div>
          </div>
        </div>
      </div>
    </nav>
  );
}
