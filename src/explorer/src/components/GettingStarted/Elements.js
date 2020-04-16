import React from 'react';
import classNames from 'classnames';

export function H1(props) {
  const classes = ['md:text-5xl', 'text-4xl', 'text-gray-800', 'leading-tight', 'font-light', 'font-extrabold', 'tracking-tight'];

  return <h1 className={classNames(classes, props.className)}>{props.children}</h1>;
}

export function H2(props) {
  const classes = ['text-3xl', 'font-thin', 'mb-3', 'ml-2'];

  return <h2 id={props.id} className={classNames(classes, props.className)}>{props.children}</h2>;
}

export function BrowserKeyExamples(props) {
  const classes = ['table-auto', 'shadow-md', 'border', 'border-gray-300'];
  return (
    <table className={classNames(classes, props.className)}>
      <thead>
        <tr>
          <th className="px-6 py-3 border-b border-gray-400 bg-gray-200 text-left text-xs leading-4 font-medium text-gray-700 uppercase tracking-wider">
            URL Pattern
          </th>
          <th className="px-6 py-3 border-b border-gray-400 bg-gray-200 text-left text-xs leading-4 font-medium text-gray-700 uppercase tracking-wider">
            Description
          </th>
        </tr>
      </thead>
      <tbody className="bg-white">
        <tr>
          <td className="border-b border-gray-200 p-2">
            <span className="text-indigo-600 block leading-loose">www.example.com</span>
            <span className="text-indigo-600 block leading-loose">www.example.com/</span>
            <span className="text-indigo-600 block leading-loose">www.example.com/*</span>
          </td>
          <td className="border-b border-gray-200 p-2">
            Matches all referrers in the domain <span className="text-indigo-600">www.example.com</span>
          </td>
        </tr>
        <tr>
          <td className="border-b border-gray-200 p-2">
            <span className="text-indigo-600">example.com/*</span>
          </td>
          <td className="border-b border-gray-200 p-2">
            Matches only referrers at <span className="text-indigo-600">example.com</span>, but no subdomains
          </td>
        </tr>
        <tr>
          <td className="border-b border-gray-200 p-2">
            <span className="text-indigo-600">*.example.com</span>
          </td>
          <td className="border-b border-gray-200 p-2">
            Matches all referrers at all subdomains of <span className="text-indigo-600">example.com</span>
          </td>
        </tr>
        <tr>
          <td className="border-b border-gray-200 p-2">
            <span className="text-indigo-600 block leading-loose">example.com/test</span>
            <span className="text-indigo-600 block leading-loose">example.com/test/</span>
            <span className="text-indigo-600 block leading-loose">example.com/test/*</span>
          </td>
          <td className="border-b border-gray-200 p-2">
            Matches all referrers in <span className="text-indigo-600">example.com/test/</span> and all subpaths
          </td>
        </tr>
      </tbody>
    </table>
  );
}

export function Card(props) {
  const classes = ['bg-gray-100', 'shadow-md', 'rounded-lg', 'border', 'border-gray-300'];

  return (
    <div className={classNames(classes, props.className)}>
      <h3 className="bg-white px-5 pt-5 text-xl uppercase font-bold text-indigo-600 rounded-lg">{props.type}</h3>
      <p className="bg-white pb-3 px-5 text-gray-600">{props.tagLine}</p>
      <div className="pt-2 pb-6 border-gray-200 border-t">
        <div className="flex flex-wrap px-5">{props.children}</div>
      </div>
    </div>
  );
}

export function CardItem(props) {
  return <div className="mx-1 px-3 py-1 my-1 bg-white border border-gray-400 rounded-full font-semibold tracking-wide">{props.children}</div>;
}

export function Link(props) {
  const classes = classNames(
    props.className,
    'hover:underline',
    'hover:text-indigo-400',
    'text-indigo-200',
    'transition',
    'ease-in-out',
    'duration-150',
    'cursor-pointer'
  );

  return (
    <a className={classes} href={props.url} rel="noopener noreferrer" target="_blank">
      {props.children}
    </a>
  );
}
