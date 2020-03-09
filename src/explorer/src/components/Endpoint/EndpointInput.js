import React from 'react';
import { TypeLabel, RequiredLabel } from './EndpointLabel';

const lookupType = docType => {
  if (docType === 'string') {
    return 'text';
  }

  if (docType === 'int') {
    return 'number';
  }
};

export default function Input(props) {
  return (
    <div className="flex flex-wrap pt-3 md:justify-around md:flex-no-wrap">
      <label className="mx-2 md:ml-4 self-center pb-2 md:pb-2 text-sm leading-5 font-medium text-gray-700 lg:w-1/4 md:w-1/2">{props.name}</label>
      <input
        type={lookupType(props.type)}
        className="mx-2 md:mx-0 bg-white focus:outline-none focus:border-indigo-200 border border-gray-300 rounded md:rounded-none md:rounded-l-lg py-2 px-4 block w-full appearance-none leading-normal"
        placeholder={props.placeholder}></input>
      <div className="flex md:flex-col pt-2 md:pt-0 align-center opacity-75">
        <TypeLabel type={props.type} />
        <RequiredLabel required={props.required} />
      </div>
    </div>
  );
}
