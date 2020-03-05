import React from 'react';
import './SideNav.css';

export default function SideNav() {
  return (
    <nav className="hidden md:block flex-none w-40 pl-2 text-base md:text-sm">
      <div className="mb-8">
        <h5 className="mb-3 text-gray-500 uppercase tracking-wide font-bold text-sm">Getting Started</h5>
        <ul>
          <li className="mb-1">
            <a
              className="px-2 -mx-2 py-1 transition duration-200 ease-in-out relative block hover:translate-x-2px hover:text-gray-900 text-gray-600 font-medium"
              href="/guide">
              <span className="rounded absolute inset-0 bg-teal-200 opacity-0"></span>
              <span className="relative">Getting Started Guide</span>
            </a>
          </li>
          <li className="mb-1">
            <a
              className="px-2 -mx-2 py-1 transition duration-200 ease-in-out relative block hover:translate-x-2px hover:text-gray-900 text-gray-600 font-medium"
              href="/samples">
              <span className="rounded absolute inset-0 bg-teal-200 opacity-0"></span>
              <span className="relative">Sample Usage</span>
            </a>
          </li>
          <li className="mb-1">
            <a
              className="px-2 -mx-2 py-1 transition duration-200 ease-in-out relative block hover:translate-x-2px hover:text-gray-900 text-gray-600 font-medium"
              href="/changelog">
              <span className="rounded absolute inset-0 bg-teal-200 opacity-0"></span>
              <span className="relative">Release Notes</span>
            </a>
          </li>
          <li className="mb-1">
            <a
              className="px-2 -mx-2 py-1 transition duration-200 ease-in-out relative block hover:translate-x-2px hover:text-gray-900 text-gray-600 font-medium"
              href="/bugs">
              <span className="rounded absolute inset-0 bg-teal-200 opacity-0"></span>
              <span className="relative">Report a Problem</span>
            </a>
          </li>
          <li className="mb-1">
            <a
              className="px-2 -mx-2 py-1 transition duration-200 ease-in-out relative block hover:translate-x-2px hover:text-gray-900 text-gray-600 font-medium"
              href="/privacy">
              <span className="rounded absolute inset-0 bg-teal-200 opacity-0"></span>
              <span className="relative">Privacy Policy</span>
            </a>
          </li>
        </ul>
      </div>
      <div className="mb-8">
        <h5 className="mb-3 text-gray-500 uppercase tracking-wide font-bold text-sm">Endpoints</h5>
        <ul>
          <li className="mb-1">
            <a
              className="px-2 -mx-2 py-1 transition duration-200 ease-in-out relative block hover:translate-x-2px hover:text-gray-900 text-gray-600 font-medium"
              href="#geocoding">
              <span className="rounded absolute inset-0 bg-teal-200 opacity-0"></span>
              <span className="relative">Geocoding</span>
            </a>
          </li>
          <li className="mb-1">
            <a
              className="px-2 -mx-2 py-1 transition duration-200 ease-in-out relative block hover:translate-x-2px hover:text-gray-900 text-gray-600 font-medium"
              href="#searching">
              <span className="rounded absolute inset-0 bg-teal-200 opacity-0"></span>
              <span className="relative">Searching</span>
            </a>
          </li>
        </ul>
      </div>
    </nav>
  );
}
