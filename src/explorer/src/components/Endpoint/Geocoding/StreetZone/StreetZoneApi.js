import React, { useReducer, useEffect } from 'react';
import produce from 'immer';
import EndpointInput from '../../EndpointInput';
import EndpointSelect from '../../EndpointSelect';
import EndpointSwitch from '../../EndpointSwitch';
import EndpointAdvancedToggle from '../../EndpointAdvancedToggle';
import stringify, { hasRequiredParts } from '../../QueryString';
import schema from './meta';

const url = 'https://api.mapserv.utah.gov/api/v1/geocode/:street/:zone';
const initialState = schema.getDefault();
const charactersToWarnOn = ['#', '?', '&'];

const reducer = produce((draft, action) => {
  draft[action.type] = action.payload;

  return draft;
});

export default function StreetZone(props) {
  const [state, dispatch] = useReducer(reducer, initialState);
  const { setFetchUrl, setDisplayUrl, setInvalidCharacter } = props.urls;

  useEffect(() => {
    if (!hasRequiredParts(state, url, initialState)) {
      setDisplayUrl(null);
      setFetchUrl(null);

      return;
    }

    let specialCharacter;
    charactersToWarnOn.forEach(character => {
      if (state.street.includes(character)) {
        specialCharacter = character;
      }
    });

    setInvalidCharacter(specialCharacter);

    setDisplayUrl(stringify(state, url, initialState, 'your-api-key'));
    setFetchUrl(stringify(state, url, initialState, process.env.REACT_APP_API_KEY));
  }, [state, setFetchUrl, setDisplayUrl, setInvalidCharacter]);

  return (
    <>
      <EndpointInput name="street" schema={schema.fields.street.clone()} dispatch={dispatch} />
      <EndpointInput name="zone" schema={schema.fields.zone.clone()} dispatch={dispatch} />
      <EndpointAdvancedToggle>
        <EndpointInput name="spatialReference" schema={schema.fields.spatialReference.clone()} dispatch={dispatch} />
        <EndpointInput name="acceptScore" schema={schema.fields.acceptScore.clone()} dispatch={dispatch} />
        <EndpointSwitch name="pobox" schema={schema.fields.pobox.clone()} dispatch={dispatch} />
        <EndpointSelect name="locators" schema={schema.fields.locators.clone()} dispatch={dispatch}></EndpointSelect>
        <EndpointInput name="suggest" schema={schema.fields.suggest.clone()} dispatch={dispatch} />
        <EndpointSwitch name="scoreDifference" schema={schema.fields.scoreDifference.clone()} dispatch={dispatch} />
        <EndpointSelect dispatch={dispatch} name="format" schema={schema.fields.format.clone()}></EndpointSelect>
        <EndpointInput name="callback" schema={schema.fields.callback.clone()} dispatch={dispatch} />
      </EndpointAdvancedToggle>
    </>
  );
}
