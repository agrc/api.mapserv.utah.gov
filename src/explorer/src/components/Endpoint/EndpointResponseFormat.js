import React from 'react';
import EndpointSelect from './EndpointSelect';
import schema from './Geocoding/StreetZone/meta';

export default function Format({ dispatch }) {
  return (
    <EndpointSelect dispatch={dispatch}
      name="format"
      schema={schema.fields.format.clone()} >
      <option value="default">default</option>
      <option value="esrijson">esrijson</option>
      <option value="geojson">geojson</option>
    </EndpointSelect>
  );
}
