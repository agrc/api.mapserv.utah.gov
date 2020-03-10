import React from 'react';
import { TypeLabel, RequiredLabel } from './EndpointLabel';

export default function Field(props) {

  return (
    <div className="flex flex-wrap pt-3 md:justify-around md:flex-no-wrap">
      <label className="self-center mx-2 md:ml-4 pb-2 md:pb-2 text-sm leading-5 font-medium text-gray-700 lg:w-1/4 md:w-1/2">{props.name}</label>
      <select
        onChange={event =>
          props.dispatch({
            type: props.name,
            payload: event.target.value
          })
        }
        className="bg-white focus:outline-none focus:border-indigo-200 border border-gray-300 rounded md:rounded-none md:rounded-l-lg mx-2 md:mx-0 py-2 px-4 block w-full appearance-none leading-normal">
        {props.children}
      </select>
      <div className="flex md:flex-col pt-2 md:pt-0 align-center opacity-75">
        <TypeLabel type={props.type} />
        <RequiredLabel required={props.required} />
      </div>
    </div>
  );
}
