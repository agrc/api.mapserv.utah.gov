import React, { useState } from 'react';
import Endpoint from '../../Endpoint';
import StreetZoneApi from './StreetZoneApi';
import StreetZoneDocs from './StreetZoneDocs';

const meta = {
  description: 'Finding a spatial coordinate on the ground for an address',
  name: 'Street and Zone'
};

export default function StreetZone(props) {
  const [fetchUrl, setFetchUrl] = useState('');
  const [displayUrl, setDisplayUrl] = useState('');
  const [invalidCharacter, setInvalidCharacter] = useState();

  return (
    <Endpoint {...meta}
      displayUrl={displayUrl}
      fetchUrl={fetchUrl}
      collapsed={props.collapsed}
      invalidCharacter={invalidCharacter}>
      <StreetZoneApi urls={{ setFetchUrl, setDisplayUrl, setInvalidCharacter }} key="api"></StreetZoneApi>
      <StreetZoneDocs key="docs"></StreetZoneDocs>
    </Endpoint>
  );
}
