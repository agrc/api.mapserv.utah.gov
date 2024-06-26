import React from 'react';
import classNames from 'classnames';
import { EndpointContext } from '../Endpoint';
import { useHistory, useParams } from 'react-router-dom';

function Tip(props) {
  return (
    <div className={props.className}>
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
  const node = React.useRef();

  return (
    <label className="ml-2 block text-xl leading-10 font-medium text-gray-700 group" ref={node}>
      {props.children}
      <DeepLink id={props.children.toString()} parentNode={node} />
    </label>
  );
}

export const DeepLink = ({ id, parentNode }) => {
  /* adds a clickable, deep link to a header or label that shows up on hover
  steps to add this to a component (see Label as an example):
  1. Add this component as a child of the parent node
  2. Add the `group` class to the parent node
  3. Add a ref to the parent node and pass it to this component
  4. Pass a unique id to this component, something like `props.children.toString()`
  */
  const endpointContext = React.useContext(EndpointContext);
  const { apiVersion, endpointCategory, endpoint, display } = useParams();
  const history = useHistory();

  const url = `/documentation/${apiVersion}/${endpointContext.category}/${endpointContext.id}/${endpointContext.display}#${id}`;

  const onClick = () => {
    history.replace(url);

    scroll();
  };

  const scroll = React.useCallback(() => {
    parentNode.current.scrollIntoView({ behavior: 'smooth' });
  }, [parentNode]);

  React.useEffect(() => {
    if (endpointCategory === endpointContext.category &&
      endpoint === endpointContext.id &&
      display === endpointContext.display &&
      decodeURIComponent(history.location.hash) === `#${id}`) {

      window.addEventListener('load', () => {
        scroll();
      });
    }
  }, [id, display, endpoint, endpointCategory, endpointContext, history.location.hash, scroll]);

  return (
    <svg
      id={id}
      aria-hidden={true}
      onClick={onClick}
      className="hidden group-hover:inline-block hover:cursor-pointer cursor-pointer"
      width="1em"
      height="1em"
      viewBox="0 0 16 16"
      fill="currentColor"
      xmlns="http://www.w3.org/2000/svg">
      <path d="M4.715 6.542L3.343 7.914a3 3 0 1 0 4.243 4.243l1.828-1.829A3 3 0 0 0 8.586 5.5L8 6.086a1.001 1.001 0 0 0-.154.199 2 2 0 0 1 .861 3.337L6.88 11.45a2 2 0 1 1-2.83-2.83l.793-.792a4.018 4.018 0 0 1-.128-1.287z" />
      <path d="M6.586 4.672A3 3 0 0 0 7.414 9.5l.775-.776a2 2 0 0 1-.896-3.346L9.12 3.55a2 2 0 0 1 2.83 2.83l-.793.792c.112.42.155.855.128 1.287l1.372-1.372a3 3 0 0 0-4.243-4.243L6.586 4.672z" />
    </svg>
  );
};

function Description(props) {
  return <div className="ml-4">{props.children}</div>;
}

function Heading(props) {
  const classes = classNames(props.className, 'text-gray-500', 'text-3xl', 'font-thin', 'tracking-tight', 'group');
  const node = React.useRef();

  return (
    <h2 className={classes} ref={node}>
      {props.children}
      <DeepLink id={props.children.toString()} parentNode={node} />
    </h2>
  );
}

function SubHeading(props) {
  const node = React.useRef();

  return (
    <h4 className="text-gray-500 text-2xl font-thin tracking-tight leading-10 ml-4 mt-4 group" ref={node}>
      {props.children}
      <DeepLink id={props.children.toString()} parentNode={node} />
    </h4>
  );
}

export { Description, Heading, SubHeading, Tip, Code, Link, TipLink, Label };
