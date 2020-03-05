import React, { useState } from 'react';
import Endpoint from './Endpoint';
import EndpointInput from './EndpointInput';
import EndpointSelect from './EndpointSelect';
import EndpointSwitch from './EndpointSwitch';

export default function StreetZone() {
  const [advanced, setAdvanced] = useState(false);

  return (
    <Endpoint name="Street and Zone" description="Finding a spatial coordinate on the ground for an address">
      <EndpointInput name="street" placeholder="123 main street" type="string" required="true" />
      <EndpointInput name="zone" placeholder="SLC" type="string" required={true} />
      <div className="flex items-center cursor-pointer pt-3 justify-between lg:justify-start" onClick={() => setAdvanced(!advanced)}>
        <h4 className="lg:w-1/5 -ml-3 text-gray-500 text-xl font-hairline tracking-tight">Advanced Usage</h4>
        <svg
          className="h-5 w-5 fill-current text-indigo-600 hover:text-indigo-500 transition duration-150 ease-in-out"
          viewBox="0 0 20 20"
          xmlns="http://www.w3.org/2000/svg">
          <path
            d="M17 16h2v-3h-6v3h2v4h2v-4zM1 9h6v3H1V9zm6-4h6v3H7V5zM3 0h2v8H3V0zm12 0h2v12h-2V0zM9 0h2v4H9V0zM3 12h2v8H3v-8zm6-4h2v12H9V8z"
            fill-rule="evenodd"
          />
        </svg>
      </div>
      {advanced ? (
        <>
          <EndpointInput name="spatialReference" placeholder="26912" type="int" required={false} />
          <EndpointInput name="acceptScore" placeholder="70" type="int" required={false} />
          <EndpointSwitch name="pobox" placeholder="true" type="boolean" required={false} />
          <EndpointSelect name="locators" placeholder="all" type="string" required={false}>
            <option selected>all</option>
            <option>addressPoints</option>
            <option>roadCenterlines</option>
          </EndpointSelect>
          <EndpointInput name="suggest" placeholder="0" type="int" required={false} />
          <EndpointSwitch name="scoreDifference" placeholder="false" type="boolean" required={false} />
          <EndpointSelect name="format" placeholder="default" type="string" required={false}>
            <option disabled selected></option>
            <option>esrijson</option>
            <option>geojson</option>
          </EndpointSelect>
          <EndpointInput name="callback" type="string" required={false} />
        </>
      ) : null}
    </Endpoint>
  );
}
