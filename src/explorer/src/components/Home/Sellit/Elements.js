import React from 'react';
import className from 'classnames';

export function H1(props) {
  const classes = ['text-3xl', 'font-light', 'text-gray-700', 'col-span-2', 'lowercase'];

  return <h1 className={className(classes, props.className)}>{props.children}</h1>;
}
