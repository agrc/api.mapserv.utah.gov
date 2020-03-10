import React from 'react';
import EndpointSelect from './EndpointSelect';

export default function Format(props) {
  return (
    <EndpointSelect
      onChange={event =>
        props.dispatch({
          type: 'format',
          payload: event.target.value
        })
      }
      name="format"
      placeholder="default"
      type="string"
      required={false}>
      <option disabled selected></option>
      <option value="esrijson">esrijson</option>
      <option value="geojson">geojson</option>
    </EndpointSelect>
  );
}
