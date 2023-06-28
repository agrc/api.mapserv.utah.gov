import React from 'react';
import CallToAction from '../../Action';

export default function Landing() {
  return (
    <header className="text-center mx-auto max-w-3xl">
      <h1 className="md:text-5xl text-3xl text-gray-800 leading-tight font-light font-extrabold tracking-tight">
        Data to enrich your <span className="text-indigo-600">business</span>
      </h1>
      <h2 className="md:text-3xl text-2xl block text-gray-700 font-light tracking-tight">
        An API to access <span className="font-semibold">Utah</span> geospatial data.
      </h2>
      <p className="md:text-lg text-base mt-3 font-light text-gray-600 leading-relaxed tracking-wide">
        The Automated Geographic Reference Center (AGRC) is the State of Utah's map technology coordination office. The AGRC creates, maintains, and stores
        geospatial data in the State Geographic Information Database (SGID), a one-stop shop to hundreds of data layers developed, aggregated, or acquired by
        state government. This web API gives you completely open access to
        <span className="text-2xl block text-indigo-600 font-bold tracking-tighter">all of that data.</span>
      </p>

      <CallToAction></CallToAction>
    </header>
  );
}
