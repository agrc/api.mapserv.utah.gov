import React from 'react';
import classNames from 'classnames';

function Tip(props) {
  return (
    <div>
      <div className="p-2 bg-gray-600 text-indigo-100 leading-none flex" role="alert">
        <span className="flex self-start md:self-center rounded-full bg-gray-900 uppercase px-2 py-1 text-xs font-bold mr-3">Tip</span>
        <span className="mr-2 text-left flex-auto font-md leading-5">{props.children}</span>
      </div>
    </div>
  );
}

function Code(props) {
  const classes = classNames(props.className, 'text-indigo-900', 'bg-indigo-100', 'px-1', 'rounded');

  return <code className={classes}>{props.children}</code>;
}

function TipLink(props) {
  const classes = classNames(props.className, 'underline', 'hover:text-indigo-200', 'transition', 'ease-in-out', 'duration-150');

  return (
    <a className={classes} href={props.url} rel="noopener noreferrer" target="_blank">
      {props.children}
    </a>
  );
}

function Link(props) {
  const classes = classNames(props.className, 'hover:underline', 'hover:text-indigo-500', 'text-indigo-600', 'transition', 'ease-in-out', 'duration-150');

  return (
    <a className={classes} href={props.url} rel="noopener noreferrer" target="_blank">
      {props.children}
    </a>
  );
}

function Label(props) {
  return <label className="ml-2 block text-xl font-thin leading-10 font-medium text-gray-700">{props.children}</label>;
}

function Description(props) {
  return <div className="ml-4">{props.children}</div>;
}

function Heading(props) {
  const classes = classNames(props.className, 'text-gray-500', 'text-3xl', 'font-thin', 'tracking-tight');

  return <h2 className={classes}>{props.children}</h2>;
}

function SubHeading(props) {
  return <h4 className="text-gray-500 text-2xl font-thin tracking-tight leading-10 ml-4 mt-4">{props.children}</h4>;
}

export { Description, Heading, SubHeading, Tip, Code, Link, TipLink, Label };
