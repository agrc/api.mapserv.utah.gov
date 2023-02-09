import React from 'react';
import { NavLink, useParams } from 'react-router-dom';

export function ResponsiveSideNav() {
  const { apiVersion } = useParams();

  return (
    <nav>
      <div className="mb-8">
        <NavLink exact to="/" activeClassName="hidden" className="flex justify-center mb-6">
          <svg id="svg-logo" height="30" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 410 155">
            <g id="peak" className="fill-current text-indigo-400">
              <path d="M204.37,37.51l51.91,51.85c14.12,0.71,27.68,1.7,40.52,2.95L204.37,0,111.8,92.45c12.85-1.27,26.43-2.29,40.59-3Z"></path>
            </g>
            <g id="hills" className="fill-current text-indigo-600">
              <path d="M149.78,39.37L131.09,20.7,50,101.71c13.38-2.9,28.72-5.41,45.59-7.47Z"></path>
              <path d="M278.92,20.7l-18.7,18.67,54.21,54.86c16.88,2.06,32.22,4.58,45.59,7.47Z"></path>
            </g>
            <g id="land" className="fill-current text-indigo-700">
              <path d="M370.39,114.15c-30.68-7.7-74.28-13.22-124.17-15.21L203.88,56.66,161.46,99c-50.24,2.12-93.95,7.83-124.26,15.74-17,4.43-29.84,9.57-37.19,15.15,22.44-5.19,52.15-9.49,86.72-12.5,23.16-2,48.51-3.46,75.31-4.22L203.88,155l41.92-41.87c27.61,0.74,53.7,2.2,77.48,4.28,34.57,3,64.28,7.31,86.72,12.5-7.7-5.84-21.38-11.19-39.61-15.77M220.9,107.92l-14.55,14.54-2.43,19.6-2.62-19.71-14.06-14-21.4-2.65,22-2.91L201.5,89.12,203.88,70l2.56,19.33,13.21,13.19,21.86,2.71Z"></path>
            </g>
            <g id="dots" className="fill-current text-indigo-300">
              <path d="M8.36,133.81l1.09,7.44,13.35-1.94-1.09-7.44Zm22.16-3.17,0.9,7.46,13.39-1.62L43.91,129Zm22.12-2.51,0.72,7.48,13.43-1.3-0.72-7.48ZM74.73,126l0.61,7.49,13.45-1.1-0.61-7.49ZM97,124.24l0.53,7.5,13.46-1-0.53-7.5Zm22.2-1.54,0.38,7.51,13.48-.68-0.38-7.5ZM155,121.07l-13.48.47,0.27,7.51,13.48-.47Z"></path>
              <path d="M385.76,131.87l-1.08,7.44L398,141.25l1.09-7.44ZM363.55,129l-0.9,7.46L376,138.11l0.91-7.46Zm-22.15-2.19-0.72,7.48,13.43,1.3,0.73-7.48Zm-22.12-2-0.61,7.49,13.46,1.1,0.61-7.49ZM297,123.29l-0.53,7.5,13.46,1,0.54-7.5ZM274.83,122l-0.38,7.5,13.48,0.68,0.38-7.51Zm-22.61,6.56L265.71,129l0.27-7.51-13.48-.47Z"></path>
            </g>
          </svg>
        </NavLink>
        <h5 className="mb-3 text-gray-500 uppercase tracking-wide font-bold text-sm">Information</h5>
        <ul>
          <li className="mb-1">
            <NavLink
              exact
              to="/documentation"
              activeClassName="text-indigo-600 font-bold"
              className="px-2 -mx-2 py-1 transition duration-200 ease-in-out relative block hover:translate-x-2px hover:text-gray-900 text-gray-600 font-medium">
              Documentation
            </NavLink>
          </li>
          <li className="mb-1">
            <NavLink
              to="/getting-started"
              className="px-2 -mx-2 py-1 transition duration-200 ease-in-out relative block hover:translate-x-2px hover:text-gray-900 text-gray-600 font-medium">
              Getting Started Guide
            </NavLink>
          </li>
          <li className="mb-1">
            <a
              className="px-2 -mx-2 py-1 transition duration-200 ease-in-out relative block hover:translate-x-2px hover:text-gray-900 text-gray-600 font-medium"
              href="https://github.com/agrc/api.mapserv.utah.gov/tree/development/samples">
              <span className="relative">Sample Usage</span>
            </a>
          </li>
          <li className="mb-1">
            <a
              className="px-2 -mx-2 py-1 transition duration-200 ease-in-out relative block hover:translate-x-2px hover:text-gray-900 text-gray-600 font-medium"
              href="https://github.com/agrc/api.mapserv.utah.gov/releases">
              <span className="relative">Release Notes</span>
            </a>
          </li>
          <li className="mb-1">
            <a
              className="px-2 -mx-2 py-1 transition duration-200 ease-in-out relative block hover:translate-x-2px hover:text-gray-900 text-gray-600 font-medium"
              href="https://github.com/agrc/api.mapserv.utah.gov/issues/new">
              <span className="relative">Report a Problem</span>
            </a>
          </li>
          <li className="mb-1">
            <NavLink
              to="/privacy-policy"
              className="relative px-2 -mx-2 py-1 transition duration-200 ease-in-out relative block hover:translate-x-2px hover:text-gray-900 text-gray-600 font-medium">
              Privacy Policy
            </NavLink>
          </li>
        </ul>
      </div>
      <div className="mb-8">
        <h5 className="mb-3 text-gray-500 uppercase tracking-wide font-bold text-sm">Endpoints</h5>
        <ul>
          <li className="mb-1">
            <NavLink
              isActive={(match) => {
                return !match;
              }}
              className="px-2 -mx-2 py-1 transition duration-200 ease-in-out relative block hover:translate-x-2px hover:text-gray-900 text-gray-600 font-medium"
              to={`/documentation/${apiVersion}/geocoding`}>
              <span className="relative">Geocoding</span>
            </NavLink>
          </li>
          <li className="mb-1">
            <NavLink
              isActive={(match) => {
                return !match;
              }}
              className="px-2 -mx-2 py-1 transition duration-200 ease-in-out relative block hover:translate-x-2px hover:text-gray-900 text-gray-600 font-medium"
              to={`/documentation/${apiVersion}/searching`}>
              <span className="relative">Searching</span>
            </NavLink>
          </li>
        </ul>
      </div>
    </nav>
  );
}

export function FixedBottomNav(props) {
  return (
    <nav className="md:hidden fixed bottom-0 w-full bg-gray-800 overflow-x-auto">
      <div className="max-w-7xl mx-auto px-2 sm:px-6 lg:px-8">
        <div className="relative flex items-center justify-between h-16">
          <div className="flex-1 flex items-center justify-center sm:items-stretch sm:justify-start">
            <div className="sm:ml-6">
              <div className="flex">
                <NavLink
                  exact
                  to="/"
                  activeClassName="hidden"
                  className="px-3 py-2 rounded-md text-sm font-medium leading-5 text-gray-300 hover:text-white hover:bg-gray-700 focus:outline-none focus:text-white focus:bg-gray-700 transition duration-150 ease-in-out">
                  <svg id="svg-logo" height="21" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 410 155">
                    <g id="peak" className="fill-current text-indigo-400">
                      <path d="M204.37,37.51l51.91,51.85c14.12,0.71,27.68,1.7,40.52,2.95L204.37,0,111.8,92.45c12.85-1.27,26.43-2.29,40.59-3Z"></path>
                    </g>
                    <g id="hills" className="fill-current text-indigo-600">
                      <path d="M149.78,39.37L131.09,20.7,50,101.71c13.38-2.9,28.72-5.41,45.59-7.47Z"></path>
                      <path d="M278.92,20.7l-18.7,18.67,54.21,54.86c16.88,2.06,32.22,4.58,45.59,7.47Z"></path>
                    </g>
                    <g id="land" className="fill-current text-white">
                      <path d="M370.39,114.15c-30.68-7.7-74.28-13.22-124.17-15.21L203.88,56.66,161.46,99c-50.24,2.12-93.95,7.83-124.26,15.74-17,4.43-29.84,9.57-37.19,15.15,22.44-5.19,52.15-9.49,86.72-12.5,23.16-2,48.51-3.46,75.31-4.22L203.88,155l41.92-41.87c27.61,0.74,53.7,2.2,77.48,4.28,34.57,3,64.28,7.31,86.72,12.5-7.7-5.84-21.38-11.19-39.61-15.77M220.9,107.92l-14.55,14.54-2.43,19.6-2.62-19.71-14.06-14-21.4-2.65,22-2.91L201.5,89.12,203.88,70l2.56,19.33,13.21,13.19,21.86,2.71Z"></path>
                    </g>
                    <g id="dots" className="fill-current text-indigo-300">
                      <path d="M8.36,133.81l1.09,7.44,13.35-1.94-1.09-7.44Zm22.16-3.17,0.9,7.46,13.39-1.62L43.91,129Zm22.12-2.51,0.72,7.48,13.43-1.3-0.72-7.48ZM74.73,126l0.61,7.49,13.45-1.1-0.61-7.49ZM97,124.24l0.53,7.5,13.46-1-0.53-7.5Zm22.2-1.54,0.38,7.51,13.48-.68-0.38-7.5ZM155,121.07l-13.48.47,0.27,7.51,13.48-.47Z"></path>
                      <path d="M385.76,131.87l-1.08,7.44L398,141.25l1.09-7.44ZM363.55,129l-0.9,7.46L376,138.11l0.91-7.46Zm-22.15-2.19-0.72,7.48,13.43,1.3,0.73-7.48Zm-22.12-2-0.61,7.49,13.46,1.1,0.61-7.49ZM297,123.29l-0.53,7.5,13.46,1,0.54-7.5ZM274.83,122l-0.38,7.5,13.48,0.68,0.38-7.51Zm-22.61,6.56L265.71,129l0.27-7.51-13.48-.47Z"></path>
                    </g>
                  </svg>
                </NavLink>
                {props.children}
                <NavLink
                  isActive={(match) => {
                    return !match;
                  }}
                  activeClassName="hidden"
                  className="px-2 -mx-2 py-1 transition duration-200 ease-in-out relative block hover:translate-x-2px hover:text-gray-900 text-gray-600 font-medium"
                  to="/documentation#geocoding">
                  <span className="relative">Geocoding</span>
                </NavLink>
                <NavLink
                  isActive={(match) => {
                    return !match;
                  }}
                  activeClassName="hidden"
                  className="px-2 -mx-2 py-1 transition duration-200 ease-in-out relative block hover:translate-x-2px hover:text-gray-900 text-gray-600 font-medium"
                  to="/documentation#searching">
                  <span className="relative">Searching</span>
                </NavLink>
              </div>
            </div>
          </div>
        </div>
      </div>
    </nav>
  );
}

export function InlineTopNav(props) {
  return (
    <nav className="w-full bg-gray-800 overflow-x-auto">
      <div className="max-w-7xl mx-auto px-2 sm:px-6 lg:px-8">
        <div className="relative flex items-center justify-between h-16">
          <div className="flex-1 flex items-center justify-center sm:items-stretch sm:justify-start">
            <div className="sm:ml-6">
              <div className="flex">
                <NavLink
                  to="/"
                  className="px-3 py-2 rounded-md text-sm font-medium leading-5 text-gray-300 hover:text-white hover:bg-gray-700 focus:outline-none focus:text-white focus:bg-gray-700 transition duration-150 ease-in-out">
                  <svg id="svg-logo" height="21" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 410 155">
                    <g id="peak" className="fill-current text-indigo-400">
                      <path d="M204.37,37.51l51.91,51.85c14.12,0.71,27.68,1.7,40.52,2.95L204.37,0,111.8,92.45c12.85-1.27,26.43-2.29,40.59-3Z"></path>
                    </g>
                    <g id="hills" className="fill-current text-indigo-600">
                      <path d="M149.78,39.37L131.09,20.7,50,101.71c13.38-2.9,28.72-5.41,45.59-7.47Z"></path>
                      <path d="M278.92,20.7l-18.7,18.67,54.21,54.86c16.88,2.06,32.22,4.58,45.59,7.47Z"></path>
                    </g>
                    <g id="land" className="fill-current text-white">
                      <path d="M370.39,114.15c-30.68-7.7-74.28-13.22-124.17-15.21L203.88,56.66,161.46,99c-50.24,2.12-93.95,7.83-124.26,15.74-17,4.43-29.84,9.57-37.19,15.15,22.44-5.19,52.15-9.49,86.72-12.5,23.16-2,48.51-3.46,75.31-4.22L203.88,155l41.92-41.87c27.61,0.74,53.7,2.2,77.48,4.28,34.57,3,64.28,7.31,86.72,12.5-7.7-5.84-21.38-11.19-39.61-15.77M220.9,107.92l-14.55,14.54-2.43,19.6-2.62-19.71-14.06-14-21.4-2.65,22-2.91L201.5,89.12,203.88,70l2.56,19.33,13.21,13.19,21.86,2.71Z"></path>
                    </g>
                    <g id="dots" className="fill-current text-indigo-300">
                      <path d="M8.36,133.81l1.09,7.44,13.35-1.94-1.09-7.44Zm22.16-3.17,0.9,7.46,13.39-1.62L43.91,129Zm22.12-2.51,0.72,7.48,13.43-1.3-0.72-7.48ZM74.73,126l0.61,7.49,13.45-1.1-0.61-7.49ZM97,124.24l0.53,7.5,13.46-1-0.53-7.5Zm22.2-1.54,0.38,7.51,13.48-.68-0.38-7.5ZM155,121.07l-13.48.47,0.27,7.51,13.48-.47Z"></path>
                      <path d="M385.76,131.87l-1.08,7.44L398,141.25l1.09-7.44ZM363.55,129l-0.9,7.46L376,138.11l0.91-7.46Zm-22.15-2.19-0.72,7.48,13.43,1.3,0.73-7.48Zm-22.12-2-0.61,7.49,13.46,1.1,0.61-7.49ZM297,123.29l-0.53,7.5,13.46,1,0.54-7.5ZM274.83,122l-0.38,7.5,13.48,0.68,0.38-7.51Zm-22.61,6.56L265.71,129l0.27-7.51-13.48-.47Z"></path>
                    </g>
                  </svg>
                </NavLink>
                <a
                  href="https://developer.mapserv.utah.gov"
                  className="flex-0 ml-6 px-3 py-2 rounded-md text-sm font-medium leading-5 text-white bg-gray-900 focus:outline-none focus:text-white focus:bg-gray-700 transition duration-150 ease-in-out">
                  Sign up
                </a>
                {props.children}
              </div>
            </div>
          </div>
        </div>
      </div>
    </nav>
  );
}

export function MyNavLink(props) {
  return (
    <NavLink
      exact
      to={props.to}
      activeClassName="text-indigo-600 font-bold"
      className="flex-0 ml-4 px-3 py-2 rounded-md text-sm font-medium leading-5 text-gray-300 hover:text-white hover:bg-gray-700 focus:outline-none focus:text-white focus:bg-gray-700 transition duration-150 ease-in-out">
      {props.children}
    </NavLink>
  );
}

export function CommonLinks(props) {
  return (<>
    <MyNavLink to="/documentation">Documentation</MyNavLink>
    <MyNavLink to="/getting-started">Getting Started Guide</MyNavLink>
    <MyNavLink to="/privacy-policy">Privacy Policy</MyNavLink>
  </>);
}
