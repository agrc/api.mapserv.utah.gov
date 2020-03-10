import React, { useReducer, useEffect } from 'react';
import produce from 'immer';
import EndpointInput from '../../EndpointInput';
import EndpointSelect from '../../EndpointSelect';
import EndpointSwitch from '../../EndpointSwitch';
import EndpointResponseFormat from '../../EndpointResponseFormat';
import EndpointAdvancedToggle from '../../EndpointAdvancedToggle';
import stringify, { hasRequiredParts } from '../../QueryString';
import { initialState, defaultAttributes } from './meta';

const url = 'https://api.mapserv.utah.gov/api/v1/geocode/:street/:zone';

const reducer = produce((draft, action) => {
  draft[action.type] = action.payload;

  return draft;
});

export default function StreetZone(props) {
  const [state, dispatch] = useReducer(reducer, initialState);
  const { setUrl } = props;
  useEffect(() => {
    if (!hasRequiredParts(state, url, initialState)) {
      setUrl(null);

      return;
    }

    setUrl(stringify(state, url, initialState));
  }, [state, setUrl]);

  return (
    <>
      <EndpointInput name="street" {...defaultAttributes.street} dispatch={dispatch} />
      <EndpointInput name="zone" {...defaultAttributes.zone} dispatch={dispatch} />
      <EndpointAdvancedToggle>
        <EndpointInput name="spatialReference" {...defaultAttributes.sr} dispatch={dispatch} />
        <EndpointInput name="acceptScore" {...defaultAttributes.acceptScore} dispatch={dispatch} />
        <EndpointSwitch name="pobox" {...defaultAttributes.pobox} dispatch={dispatch} />
        <EndpointSelect name="locators" {...defaultAttributes.locators} dispatch={dispatch}>
          <option value="all">all</option>
          <option value="addressPoints">addressPoints</option>
          <option value="roadCenterlines">roadCenterlines</option>
        </EndpointSelect>
        <EndpointInput name="suggest" {...defaultAttributes.suggest} dispatch={dispatch} />
        <EndpointSwitch name="scoreDifference" {...defaultAttributes.scoreDifference} dispatch={dispatch} />
        <EndpointResponseFormat dispatch={dispatch} />
        <EndpointInput name="callback" {...defaultAttributes.callback} dispatch={dispatch} />
      </EndpointAdvancedToggle>
    </>
  );
}
