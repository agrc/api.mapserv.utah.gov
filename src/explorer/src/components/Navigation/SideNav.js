import React from 'react';
import { Link } from 'react-router-dom';

const nav = 'hidden md:block flex-none w-40 pl-2 text-base md:text-sm md:pt-2';
export default function SideNav() {
  return (
    <nav>
      <div className="mb-8">
        <h5 className="mb-3 text-gray-500 uppercase tracking-wide font-bold text-sm">Getting Started</h5>
        <ul>
          <li className="mb-1">
            <Link
              to="/documentation"
              className="relative px-2 -mx-2 py-1 transition duration-200 ease-in-out relative block hover:translate-x-2px hover:text-gray-900 text-gray-600 font-medium">
              Documentation
            </Link>
          </li>
          <li className="mb-1">
            <Link
              to="/getting-started"
              className="relative px-2 -mx-2 py-1 transition duration-200 ease-in-out relative block hover:translate-x-2px hover:text-gray-900 text-gray-600 font-medium">
              Getting Started Guide
            </Link>
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
            <Link
              to="/privacy-policy"
              className="relative px-2 -mx-2 py-1 transition duration-200 ease-in-out relative block hover:translate-x-2px hover:text-gray-900 text-gray-600 font-medium">
              Privacy Policy
            </Link>
          </li>
        </ul>
      </div>
      <div className="mb-8">
        <h5 className="mb-3 text-gray-500 uppercase tracking-wide font-bold text-sm">Endpoints</h5>
        <ul>
          <li className="mb-1">
            <Link
              className="px-2 -mx-2 py-1 transition duration-200 ease-in-out relative block hover:translate-x-2px hover:text-gray-900 text-gray-600 font-medium"
              to="/documentation#geocoding">
              <span className="relative">Geocoding</span>
            </Link>
          </li>
          <li className="mb-1">
            <a
              className="px-2 -mx-2 py-1 transition duration-200 ease-in-out relative block hover:translate-x-2px hover:text-gray-900 text-gray-600 font-medium"
              href="#searching">
              <span className="relative">Searching</span>
            </a>
          </li>
        </ul>
      </div>
    </nav>
  );
}
