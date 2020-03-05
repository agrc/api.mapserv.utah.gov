import React from 'react';
import classNames from 'classnames';

function TypeLabel(props) {
  if (!['string', 'int', 'boolean'].includes(props.type)) {
    throw new Error(`field type '${props.type}' is incorrect`);
  }

  return (
    <span className="inline-flex px-2 text-xs leading-5 font-semibold rounded md:rounded-none md:rounded-tr bg-blue-100 text-blue-800 border md:border-b-0 border-blue-700">
      {props.type}
    </span>
  );
}

function RequiredLabel(props) {
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

  return <span className={classes}>{props.required ? 'required' : 'optional'}</span>;
}

export { TypeLabel, RequiredLabel };
