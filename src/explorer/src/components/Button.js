import React from 'react';
import classNames from 'classnames';

export default function Button(props) {
  const btn = [
    'bg-indigo-600',
    'border-transparent',
    'border',
    { 'cursor-not-allowed': props.disabled },
    { 'opacity-50': props.disabled },
    'duration-150',
    'ease-in-out',
    'flex',
    'focus:shadow-outline',
    'font-medium',
    { 'hover:bg-indigo-500': !props.disabled },
    'items-center',
    'leading-6',
    'px-5',
    'py-3',
    'rounded-md',
    'text-base',
    'text-white',
    'transition'
  ];

  const classes = classNames(btn, props.className);

  return (
    <button {...props} className={classes}>
      {props.children}
    </button>
  );
}
