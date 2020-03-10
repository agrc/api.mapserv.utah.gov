import React, {useState} from 'react';
import Endpoint from '../../Endpoint';
import StreetZoneApi from './StreetZoneApi';
import StreetZoneDocs from './StreetZoneDocs';

export default function StreetZone(props) {
  const [url, setUrl] = useState('');

  return (
    <Endpoint url={url} collapsed={props.collapsed} name="Street and Zone" description="Finding a spatial coordinate on the ground for an address">
      <StreetZoneApi setUrl={setUrl} key="api"></StreetZoneApi>
      <StreetZoneDocs key="docs"></StreetZoneDocs>
    </Endpoint>
  );
}
