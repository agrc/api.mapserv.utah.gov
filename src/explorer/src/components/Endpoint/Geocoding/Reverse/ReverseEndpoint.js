import React from 'react';
import Endpoint from '../../Endpoint';
import ReverseApi from './ReverseApi';
import ReverseDocs from './ReverseDocs';


const meta = {
  description: 'Returns the reverse geocoded address',
  name: 'Reverse Geocoding'
};

const ReverseEndpoint = ({ collapsed }) => {
  const [fetchUrl, setFetchUrl] = React.useState('');
  const [displayUrl, setDisplayUrl] = React.useState('');
  const [invalidCharacter, setInvalidCharacter] = React.useState();

  return (
    <Endpoint {...meta}
      displayUrl={displayUrl}
      fetchUrl={fetchUrl}
      id="reverse"
      invalidCharacter={invalidCharacter}>
      <ReverseApi urls={{ setFetchUrl, setDisplayUrl, setInvalidCharacter }} key="api" />
      <ReverseDocs key="docs" />
    </Endpoint>
  );
};

export default ReverseEndpoint;
