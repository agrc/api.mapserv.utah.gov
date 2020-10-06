import React from 'react';
import Endpoint from '../../Endpoint';
import SearchApi from './SearchApi';
import SearchDocs from './SearchDocs';


const meta = {
  description: 'Returns geometries and attributes matching the search criteria',
  name: 'Search'
};

const SearchEndpoint = ({ collapsed }) => {
  const [fetchUrl, setFetchUrl] = React.useState('');
  const [displayUrl, setDisplayUrl] = React.useState('');
  const [invalidCharacter, setInvalidCharacter] = React.useState();

  return (
    <Endpoint {...meta}
      displayUrl={displayUrl}
      fetchUrl={fetchUrl}
      id="search"
      invalidCharacter={invalidCharacter}>
      <SearchApi urls={{ setFetchUrl, setDisplayUrl, setInvalidCharacter }} key="api" />
      <SearchDocs key="docs" />
    </Endpoint>
  );
};

export default SearchEndpoint;
