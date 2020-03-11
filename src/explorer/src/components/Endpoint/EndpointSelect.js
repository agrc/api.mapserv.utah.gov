import React from 'react';
import { TypeLabel, RequiredLabel } from './EndpointLabel';

export default function Field(props) {
  return (
    <div className="flex flex-wrap pt-3 md:justify-around md:flex-no-wrap">
      <label className="self-center mx-2 md:ml-4 pb-2 md:pb-2 text-sm leading-5 font-medium text-gray-700 lg:w-1/4 md:w-1/2">{props.name}</label>
      <div className="inline-block relative w-full">
        <select
          onChange={event =>
            props.dispatch({
              type: props.name,
              payload: event.target.value
            })
          }
          required={props.required}
          defaultValue={props.placeholder}
          className="bg-white focus:outline-none focus:border-indigo-200 border border-gray-300 md:border-r-0 rounded md:rounded-none md:rounded-l-lg mx-2 md:mx-0 py-2 px-4 block w-full appearance-none leading-normal">
          {props.children}
        </select>
        <div className="pointer-events-none absolute inset-y-0 right-0 flex items-center px-2 text-gray-700">
          <svg className="fill-current h-4 w-4" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20">
            <path d="M9.293 12.95l.707.707L15.657 8l-1.414-1.414L10 10.828 5.757 6.586 4.343 8z" />
          </svg>
        </div>
      </div>
      <div className="flex md:flex-col pt-2 md:pt-0 align-center opacity-75">
        <TypeLabel type={props.type} />
        <RequiredLabel required={props.required} />
      </div>
    </div>
  );
}
