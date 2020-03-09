import React from 'react';
import EndpointSelect from './EndpointSelect';

export default function Format() {
  return (
    <EndpointSelect name="format" placeholder="default" type="string" required={false}>
      <option disabled selected></option>
      <option>esrijson</option>
      <option>geojson</option>
    </EndpointSelect>
  );
}
