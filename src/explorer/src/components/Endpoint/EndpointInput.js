import React from 'react';
import { TypeLabel, RequiredLabel } from './EndpointLabel';


export default function Input({ schema, name, dispatch }) {
  return (
    <div className="flex flex-wrap pt-3 md:justify-around md:flex-no-wrap">
      <label className="mx-2 md:ml-4 self-center pb-2 md:pb-2 text-sm leading-5 font-medium text-gray-700 lg:w-1/4 md:w-1/2">{name}</label>
      <input
        type={schema.type}
        className="mx-2 md:mx-0 bg-white focus:outline-none focus:border-indigo-200 border border-gray-300 md:border-r-0 rounded md:rounded-none md:rounded-l-lg py-2 px-4 block w-full appearance-none leading-normal"
        placeholder={schema.getPlaceholder()}
        onChange={event =>
          dispatch({
            type: name,
            payload: event.target.value
          })
        }
        required={schema.isFieldRequired()}></input>
      <div className="flex md:flex-col pt-2 md:pt-0 align-center opacity-75">
        <TypeLabel type={schema.type} />
        <RequiredLabel required={schema.isFieldRequired()} />
      </div>
    </div>
  );
}
