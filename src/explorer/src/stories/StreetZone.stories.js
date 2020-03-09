import React from 'react';
import { StreetZoneEndpoint } from '../components/Endpoint/Geocoding';
import StreetZoneApi from '../components/Endpoint/Geocoding/StreetZone/StreetZoneApi';
import StreetZoneDocs from '../components/Endpoint/Geocoding/StreetZone/StreetZoneDocs';

export default {
  title: 'Street Zone Api',
  component: StreetZoneEndpoint
};

export const Component = () => <StreetZoneEndpoint></StreetZoneEndpoint>;
export const ComponentOpen = () => <StreetZoneEndpoint collapsed={false}></StreetZoneEndpoint>;
export const Api = () => <StreetZoneApi></StreetZoneApi>
export const Docs = () => <StreetZoneDocs></StreetZoneDocs>
