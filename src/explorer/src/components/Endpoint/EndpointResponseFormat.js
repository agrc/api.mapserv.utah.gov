import React from 'react';
import EndpointSelect from './EndpointSelect';

export default function Format(props) {
  return (
    <EndpointSelect dispatch={props.dispatch}
      name="format"
      placeholder="default"
      type="string"
      required={false} >
      <option value="default">default</option>
      <option value="esrijson">esrijson</option>
      <option value="geojson">geojson</option>
    </EndpointSelect>
  );
}
