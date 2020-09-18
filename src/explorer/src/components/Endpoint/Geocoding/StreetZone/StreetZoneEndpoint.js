import React, { useState } from 'react';
import Endpoint from '../../Endpoint';
import StreetZoneApi from './StreetZoneApi';
import StreetZoneDocs from './StreetZoneDocs';
import { useParams } from 'react-router-dom';

const meta = {
  description: 'Finding a spatial coordinate on the ground for an address',
  name: 'Street and Zone'
};

export default function StreetZone(props) {
  const { apiVersion } = useParams();
  console.log(`api version: ${apiVersion}`);

  const [fetchUrl, setFetchUrl] = useState('');
  const [displayUrl, setDisplayUrl] = useState('');

  return (
    <Endpoint {...meta} displayUrl={displayUrl} fetchUrl={fetchUrl} collapsed={props.collapsed}>
      <StreetZoneApi urls={{ setFetchUrl, setDisplayUrl }} key="api"></StreetZoneApi>
      <StreetZoneDocs key="docs"></StreetZoneDocs>
    </Endpoint>
  );
}
