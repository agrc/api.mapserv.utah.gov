import React from 'react';
import classNames from 'classnames';
import Toggle from 'react-toggle';
import './EndpointSwitch.css';

export default function Field(props) {
  const classes = classNames(
    'px-2',
    'ml-1',
    'md:ml-0',
    'inline-flex',
    'text-xs',
    'leading-5',
    'font-semibold',
    'rounded',
    'md:rounded-none',
    'md:rounded-br',
    'border',
    {
      'bg-red-100': props.required,
      'text-red-800': props.required,
      'border-red-700': props.required,
      'bg-yellow-100': !props.required,
      'text-yellow-800': !props.required,
      'border-yellow-700': !props.required
    }
  );

  return (
    <div className="flex flex-wrap pt-3 md:justify-around md:flex-no-wrap">
      <label className="self-center pb-2 md:pb-2 text-sm leading-5 font-medium text-gray-700 lg:w-1/4 md:w-1/2">{props.name}</label>
      <Toggle className="w-full self-center" defaultChecked={false} />
      <div className="flex md:flex-col pt-2 md:pt-0 align-center opacity-75">
        <span className="inline-flex px-2 text-xs leading-5 font-semibold rounded md:rounded-none md:rounded-tr bg-blue-100 text-blue-800 border md:border-b-0 border-blue-700">
          {props.type}
        </span>
        <span className={classes}>{props.required ? 'required' : 'optional'}</span>
      </div>
    </div>
  );
}
