import React from 'react';
import { Code, Tip, TipLink, Link, Label, Description, Heading, SubHeading } from '../../Documentation/Elements';

export default function StreetZone() {
  return (
    <section>
      <Tip>
        This endpoint validates that an address can exist along a road, however, there may be no structure or mail delivery at the address. This endpoint should
        not be used to validate mailing addresses.
      </Tip>
      <Tip>
        <TipLink url="https://tools.ietf.org/html/rfc3986#section-2.1">Reserved characters</TipLink>, <Code className="opacity-75">?</Code>,{' '}
        <Code className="opacity-75">#</Code>, <Code className="opacity-75">@</Code>, etc. in your data need to be escaped or the request will fail.
      </Tip>
      <Tip>
        <Code className="opacity-75">matchAddress</Code> returns the name of the address grid system for the address. For example,{' '}
        <Code className="opacity-75">"matchAddress": "10420 E Little Cottonwood Canyon, Salt Lake City"</Code> means that the address is part of the{' '}
        <strong>Salt Lake City address grid system</strong>. It is neither within the boundaries of Salt Lake City proper, nor is that the preferred mailing
        address placename.
      </Tip>
      <section className="max-w-4xl mx-2">
        <Heading>URI Format</Heading>
        <p className="pl-5 font-hairline text-base md:text-lg lg:text-2xl">
          <span className="bg-indigo-200 border border-indigo-700 py-1 px-2 rounded-full mr-4 text-base font-bold text-indigo-700 uppercase tracking-wider">GET</span>
          https://api.mapserv.utah.gov/api/v1/
          <span className="font-normal">geocode</span>/<span className="text-indigo-900 font-semibold">:street</span>/
          <span className="text-indigo-900 font-semibold">:zone</span>
        </p>
        <Heading className="mt-8">Required Fields</Heading>
        <Label>street</Label>
        <Description>
          A Utah street address. eg: <Code>326 east south temple st</Code>. A valid mailing address or structure does not need to exist at the input street to
          find a match. If the house number exists in the range of the street, a coordinate will be extrapolated from the road centerlines.
        </Description>
        <Label>zone</Label>
        <Description>
          A Utah municipality name or 5 digit zip code eg <Code>Provo</Code> or <Code>84111</Code>.
        </Description>
        <Heading className="mt-8">Optional Fields</Heading>
        <Label>spatialReference</Label>
        <Description>
          The spatial reference defines how the coordinates will represent a location on the earth defined by how the round earth was made flat. The well known
          id's (WKID) of different coordinate systems define if the coordinates will be stored as degrees of longitude and latitude, meters, feet, etc. This
          endpoint supports the WKIDs from the{' '}
          <Link url="https://desktop.arcgis.com/en/arcmap/10.5/map/projections/pdf/geographic_coordinate_systems.pdf">Geographic Coordinate System</Link>{' '}
          reference and the{' '}
          <Link url="https://desktop.arcgis.com/en/arcmap/10.5/map/projections/pdf/projected_coordinate_systems.pdf">Projected Coordinate System</Link>{' '}
          reference. UTM Zone 12 N, with the WKID of <strong className="text-indigo-900">26912</strong>, is the default. This coordinate system is the most
          accurate reflection of Utah. It is recommended to use this coordinate system if length and area calculations are important as other coordinate systems
          will skew the truth.
          <h2>Popular Spatial Reference Ids</h2>
          <div class="align-middle mx-4 mt-4 inline-block shadow overflow-hidden sm:rounded-lg">
            <table>
              <thead>
                <tr>
                  <th className="px-6 py-3 border-b border-gray-500 bg-gray-400 text-left text-xs leading-4 font-medium text-gray-700 uppercase tracking-wider">
                    WKID
                  </th>
                  <th className="px-6 py-3 border-b border-gray-500 bg-gray-400 text-left text-xs leading-4 font-medium text-gray-700 uppercase tracking-wider">
                    Name
                  </th>
                  <th className="px-6 py-3 border-b border-gray-500 bg-gray-400 text-left text-xs leading-4 font-medium text-gray-700 uppercase tracking-wider">
                    Reason
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white">
                <tr>
                  <td className="border-b border-gray-200 p-2 text-sm leading-5 font-medium text-gray-800">26912</td>
                  <td className="border-b border-gray-200 p-2 text-gray-800">UTM Zone 12 N</td>
                  <td className="border-b border-gray-200 p-2 text-gray-800">
                    This is the State standard for all Utah spatial data. It has the least distortion for the state of Utah and the most accurate length and
                    area calculations.
                  </td>
                </tr>
                <tr>
                  <td className="border-b border-gray-200 p-2 text-sm leading-5 font-medium text-gray-800">3857</td>
                  <td className="border-b border-gray-200 p-2 text-gray-800">Web Mercator</td>
                  <td className="border-b border-gray-200 p-2 text-gray-800">
                    This is the standard for all data being displayed on a web map. Most base map services are using a web mercator projection.
                  </td>
                </tr>
                <tr>
                  <td className="border-b border-gray-200 p-2 text-sm leading-5 font-medium text-gray-800">4325</td>
                  <td className="border-b border-gray-200 p-2 text-gray-800">Latitude/Longitude (WGS84)</td>
                  <td className="border-b border-gray-200 p-2 text-gray-800">
                    This is the ubiquitous standard for spatial data. Many systems understand this format which makes it a good for interoperability.
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
          <SubHeading>Reference Information</SubHeading>
          <ul className="ml-8">
            <li>
              <Link url="https://desktop.arcgis.com/en/arcmap/latest/map/projections/about-projected-coordinate-systems.htm">What are map projections?</Link>
            </li>
            <li>
              <Link url="https://desktop.arcgis.com/en/arcmap/latest/map/projections/about-projected-coordinate-systems.htm">
                What are projected coordinate systems?
              </Link>
            </li>
            <li>
              <Link url="https://desktop.arcgis.com/en/arcmap/latest/map/projections/about-geographic-coordinate-systems.htm">
                What are geographic coordinate systems?
              </Link>
            </li>
          </ul>
        </Description>
        <Label>acceptScore</Label>
        <Description>
          Every street zone geocode will return a score for the match on a scale from 0-100. The score is a rating of how confident the system is in the choice
          of coordinates based on the input. For example, misspellings in a street name, omitting a street type when multiple streets with the same name exist,
          or omitting a street direction when the street exists in multiple quadrants will cause the result to lose points. Depending on your needs, you may
          need to limit the score the system will return. The default value of <strong className="text-indigo-900">70</strong> will give acceptable results. If
          you need extra control, use the suggest and scoreDifference options
        </Description>
        <Label>format</Label>
        <Description>
          There are three output formats for the resulting street and zone geocoding. The <strong className="text-indigo-900">default</strong> being empty.{' '}
          <Code>esrijson</Code> will parse into an <Code>esri.Graphic</Code> for mapping purposes and <Code>geojson</Code> will format as a a{' '}
          <Link url="https://tools.ietf.org/html/rfc7946#section-3.2">feature</Link>. If this value is omitted, the default json will be returned.
        </Description>
        <Label>pobox</Label>
        <Description>
          This option determines if the system should find a location for P.O. Box addresses. The <strong className="text-indigo-900">default</strong> value of{' '}
          <Code>true</Code>, will return a location for a P.O. Box only when the input zone is a 5 digit zip code. The result will be where the mail is
          delivered. This could be a traditional post office, community post office, university, etc. When analyzing where people live, P.O. Box information
          will skew results since there is no correlation between where mail is delivered and where the owner of the mail liver. View the{' '}
          <Link url="https://opendata.gis.utah.gov/datasets/utah-zip-code-po-boxes">source data</Link>.
        </Description>
        <Label>locators</Label>
        <Description>
          The locators are the search engine for address data. There are three options, The <strong className="text-indigo-900">default</strong> value of{' '}
          <Code>all</Code> will use the highest score from the <Link url="https://opendata.gis.utah.gov/datasets/utah-address-points">address point</Link> and{' '}
          <Link url="https://opendata.gis.utah.gov/datasets/utah-roads">road center line</Link> data and provide the best match rate. Address point locations
          are used in the event of a tie. Address points are a work in progress with the counties to map structures or places where mail is delivered. Road
          centerlines are a dataset with every road and the range of numbers that road segment contains.
        </Description>
        <Label>suggest</Label>
        <Description>
          The <strong className="text-indigo-900">default</strong> value of <Code>0</Code> will return the highest match. To include the other candidates, set
          this value between 1-5. The candidates respect the <Code>acceptScore</Code> option.
        </Description>
        <Label>scoreDifference</Label>
        <Description>
          When suggest is set to the <strong className="text-indigo-900">default</strong> value of <Code>0</Code>, the difference in score between the top match
          and the next highest match is calculated and returned on the result object. This can help determine if there was a tie. If the value is 0, repeat the
          request with suggest > 0 and investigate the results. A common scenario to cause a 0 is when and input address of 100 main street is input. The two
          highest score matches will be 100 south main and 100 north main. The system will arbitrarily choose one because they will have the same score.
        </Description>
        <Label>callback</Label>
        <Description>
          The callback function to execute for cross domain javascript calls (jsonp). This API supports CORS and does not recommend the use of callback and
          jsonp.
        </Description>
        <footer className="ml-2 pt-2 mt-8 md:ml-0 md:flex md:justify-around">
          <div>
            <Heading>Default Response Shape</Heading>
            <pre className="ml-4">
              {JSON.stringify(
                {
                  result: {
                    location: {
                      x: 0,
                      y: 0
                    },
                    score: 100,
                    locator: '',
                    matchAddress: '',
                    inputAddress: '',
                    addressGrid: ''
                  },
                  status: 200
                },
                null,
                1
              )}
            </pre>
          </div>
          <div>
            <Heading>Error Response Shape</Heading>
            <pre className="ml-4">
              {JSON.stringify(
                {
                  status: 0,
                  message: ''
                },
                null,
                1
              )}
            </pre>
            <p className="md:max-w-xs mt-8 text-gray-700">
              <span class="text-indigo-100 rounded-full bg-indigo-500 uppercase px-2 py-1 text-xs font-bold mr-3">Tip</span>
              Be sure to inspect the failed requests as they contain useful information. <Code>401</Code> status codes equates to your API key having problems.{' '}
              <Code>404</Code> equates to the address not being found. <Code>200</Code> is successful.
            </p>
          </div>
        </footer>
      </section>
    </section>
  );
}
