import React from 'react';
import { TypeLabel, RequiredLabel } from './EndpointLabel';


export default function Input({ schema, name, dispatch }) {
  const [ validationMessage, setValidationMessage ] = React.useState();

  const onChange = event => {
    let value;
    if (event.target.value.trim() === '' && !schema.isFieldRequired()) {
      value = null;
      setValidationMessage(null);
    } else {
      try {
        value = schema.validateSync(event.target.value);
        setValidationMessage(null);

      } catch (error) {
        if (error.name === 'ValidationError') {
          value = null;
          setValidationMessage(error.errors[0]);
        } else {
          throw error;
        }
      }
    }

    dispatch({
      type: name,
      payload: value
    });
  };

  return (
    <div className="flex flex-wrap pt-3 md:justify-around md:flex-no-wrap">
      <label className="mx-2 md:ml-4 self-center pb-2 md:pb-2 text-sm leading-5 font-medium text-gray-700 lg:w-1/4 md:w-1/2">{name}</label>
      <div className="mx-2 md:mx-0 w-full flex flex-col">
        <input
          type={schema.type}
          className="bg-white focus:outline-none focus:border-indigo-200 border border-gray-300 md:border-r-0 rounded md:rounded-none md:rounded-l-lg py-2 px-4 appearance-none leading-normal w-full block"
          placeholder={schema.getPlaceholder()}
          onChange={onChange}
          required={schema.isFieldRequired()}></input>
        { (validationMessage && validationMessage.length) ?
          <div className="text-red-400 text-sm">{validationMessage}</div> : null}
      </div>
      <div className="flex md:flex-col pt-2 md:pt-0 align-center opacity-75">
        <TypeLabel type={schema.type} />
        <RequiredLabel required={schema.isFieldRequired()} />
      </div>
    </div>
  );
}
