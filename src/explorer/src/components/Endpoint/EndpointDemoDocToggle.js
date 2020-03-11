import React from 'react';
import classNames from 'classnames';

const base = [
  'text-center',
  'text-sm',
  'tracking-wider',
  'uppercase',
  'block',
  'border',
  'border-indigo-600',
  'py-1',
  'px-4',
  'duration-150',
  'transform',
  'ease-in-out',
  'focus:outline-none',
  'shadow'
];

const normalCss = ['bg-gray-100', 'hover:bg-indigo-500', 'text-indigo-600', 'hover:text-white'];
const activeCss = ['bg-indigo-500', 'text-white'];

export default function Toggle(props) {
  const activeApi = classNames('rounded-bl-lg', base, activeCss);
  const normalApi = classNames('rounded-bl-lg', base, normalCss);

  const activeDoc = classNames('rounded-tr-lg', 'border-l-none', base, activeCss);
  const normalDoc = classNames('rounded-tr-lg', 'border-l-none', base, normalCss);

  return (
    <div className="bg-gray-200 shadow-inner border-b border-gray-300">
      <ul className="flex justify-center p-3">
        <li>
          <button type="button" className={props.active === 'api' ? activeApi : normalApi} onClick={() => props.showApi(true)}>
            demo
          </button>
        </li>
        <li>
          <button type="button" className={props.active === 'docs' ? activeDoc : normalDoc} onClick={() => props.showApi(false)}>
            documentation
          </button>
        </li>
      </ul>
    </div>
  );
}
