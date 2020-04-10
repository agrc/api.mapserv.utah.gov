import React from 'react';
import className from 'classnames';

export function P(props) {
  const classes = ['mt-0', 'text-lg'];

  return <p className={className(classes, props.className)}>{props.children}</p>;
}
