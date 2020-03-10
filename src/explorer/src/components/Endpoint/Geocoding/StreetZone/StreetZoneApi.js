import React, { useReducer, useEffect } from 'react';
import produce from 'immer';
import EndpointInput from '../../EndpointInput';
import EndpointSelect from '../../EndpointSelect';
import EndpointSwitch from '../../EndpointSwitch';
import EndpointResponseFormat from '../../EndpointResponseFormat';
import EndpointAdvancedToggle from '../../EndpointAdvancedToggle';
import stringify, { hasRequiredParts } from '../../QueryString';

const reducer = produce((draft, action) => {
  draft[action.type] = action.payload;

  return draft;
});

const initialState = {
  street: '',
  zone: '',
  spatialReference: 26912,
  acceptScore: 70,
  pobox: false,
  locators: '',
  format: '',
  suggest: 0,
  scoreDifference: false,
  callback: ''
};

const options = {
  street: {
    placeholder: '123 main street',
    type: 'string',
    required: true
  },
  zone: {
    placeholder: 'SLC',
    type: 'string',
    required: true
  },
  sr: {
    placeholder: 26912,
    type: 'int',
    required: false
  },
  acceptScore: {
    placeholder: 70,
    type: 'int',
    required: false
  },
  pobox: {
    type: 'boolean',
    required: false
  },
  locators: {
    placeholder: 'all',
    type: 'string',
    required: false
  },
  suggest: {
    placeholder: 0,
    type: 'int',
    required: false
  },
  scoreDifference: {
    placeholder: 0,
    type: 'int',
    required: false
  },
  callback: {
    placeholder: 'callback',
    type: 'string',
    required: false
  }
};

const url = 'https://api.mapserv.utah.gov/api/v1/geocode/:street/:zone';

export default function StreetZone() {
  const [state, dispatch] = useReducer(reducer, initialState);

  useEffect(() => {
    if (!hasRequiredParts(state, url, initialState)) {
      return;
    }

    console.log(stringify(state, url, initialState));
  }, [state]);

  return (
    <>
      <EndpointInput name="street" {...options.street} dispatch={dispatch} />
      <EndpointInput name="zone" {...options.zone} dispatch={dispatch} />
      <EndpointAdvancedToggle>
        <EndpointInput name="spatialReference" {...options.sr} dispatch={dispatch} />
        <EndpointInput name="acceptScore" {...options.acceptScore} dispatch={dispatch} />
        <EndpointSwitch name="pobox" {...options.pobox} dispatch={dispatch} />
        <EndpointSelect name="locators" {...options.locators} dispatch={dispatch}>
          <option selected value="all">
            all
          </option>
          <option value="addressPoints">addressPoints</option>
          <option value="roadCenterlines">roadCenterlines</option>
        </EndpointSelect>
        <EndpointInput name="suggest" {...options.suggest} dispatch={dispatch} />
        <EndpointSwitch name="scoreDifference" {...options.scoreDifference} dispatch={dispatch} />
        <EndpointResponseFormat dispatch={dispatch} />
        <EndpointInput name="callback" {...options.callback} dispatch={dispatch} />
      </EndpointAdvancedToggle>
    </>
  );
}
