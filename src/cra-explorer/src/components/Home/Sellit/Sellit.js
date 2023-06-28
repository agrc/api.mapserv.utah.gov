import React from 'react';
import { H1 } from './Elements';
import { P } from '../../Elements';

export default function Header() {
  return (
    <section>
      <div className="text-gray-600 leading-relaxed tracking-wide mb-8">
        <H1>Simple, Spatial</H1>
        <P className="ml-3">
          You can learn so much from a house address once it has been geocoded. When you geocode data, you unlock the ability to visualize the data in a
          different way. And you can use our search endpoint to answer endless questions about your addresses or location.
        </P>
        <H1 className="mt-5">Trust the data</H1>
        <P className="ml-3">
          AGRC was developed specifically to coordinate Utah-specific data, and we've been doing this for 30+ years. Other programs can provide you with similar
          data, but if you're looking for information that considers the nuances of Utah, this is it—we have the most comprehensive Utah geospatial data, and we
          want to share it with you.
        </P>
      </div>
    </section>
  );
}
