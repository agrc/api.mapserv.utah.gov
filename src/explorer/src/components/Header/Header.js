import React from 'react';

import CallToAction from '../Action';

export default function Header() {
  return (
    <header className="px-2 lg:text-center">
      <p className="text-base leading-6 text-indigo-600 font-semibold tracking-wide uppercase">AGRC Web API Explorer</p>
      <h3 className="mt-2 text-3xl leading-8 font-extrabold tracking-tight text-gray-900 sm:text-4xl">
        Data to enrich your <span className="text-indigo-600">business</span>
      </h3>
      <p className="mt-4 max-w-xl text-xl leading-4 text-gray-500 lg:mx-auto">Explore the AGRC Web API.</p>

      <CallToAction></CallToAction>
    </header>
  );
}
