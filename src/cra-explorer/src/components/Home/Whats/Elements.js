import React from 'react';
import className from 'classnames';

export function H1(props) {
  const classes = ['text-2xl', 'font-light', 'text-indigo-600', 'col-span-2'];

  return <h1 className={className(classes, props.className)}>{props.children}</h1>;
}
