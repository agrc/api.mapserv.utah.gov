import React from 'react';
import classNames from 'classnames';

function TypeLabel({ type }) {
  if (!['string', 'number', 'boolean'].includes(type)) {
    throw new Error(`field type '${type}' is incorrect`);
  }

  return (
    <span className="ml-4 md:m-0 md:mr-2 inline-flex px-2 text-xs leading-5 font-semibold rounded md:rounded-none md:rounded-tr bg-blue-100 text-blue-800 border border-blue-200">
      {type}
    </span>
  );
}

function RequiredLabel({ required }) {
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
    'border-gray-300',
    'bg-white',
    {
      'text-red-800': required,
      'text-gray-800': !required
    }
  );

  return <span className={classes}>{required ? 'required' : 'optional'}</span>;
}

export { TypeLabel, RequiredLabel };
