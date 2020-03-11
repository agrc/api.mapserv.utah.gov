import React from 'react';
import classNames from 'classnames';

function TypeLabel(props) {
  if (!['string', 'int', 'boolean'].includes(props.type)) {
    throw new Error(`field type '${props.type}' is incorrect`);
  }

  return (
    <span className="ml-4 md:m-0 md:mr-2 inline-flex px-2 text-xs leading-5 font-semibold rounded md:rounded-none md:rounded-tr bg-blue-100 text-blue-800 border border-blue-200">
      {props.type}
    </span>
  );
}

function RequiredLabel(props) {
  const classes = classNames(
    'md:m-0',
    'md:mr-2',
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
    'md:border-t-0',
    {
      'bg-red-100': props.required,
      'text-red-800': props.required,
      'border-red-200': props.required,
      'bg-gray-100': !props.required,
      'text-gray-800': !props.required,
      'border-gray-300': !props.required
    }
  );

  return <span className={classes}>{props.required ? 'required' : 'optional'}</span>;
}

export { TypeLabel, RequiredLabel };
