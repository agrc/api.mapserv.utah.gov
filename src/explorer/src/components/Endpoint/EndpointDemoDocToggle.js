import React from 'react';
import classNames from 'classnames';

const base = ['text-center', 'block', 'border', 'border-indigo-600', 'py-2', 'px-4', 'duration-150', 'transform', 'ease-in-out', 'focus:outline-none'];

const normalCss = ['hover:bg-indigo-500', 'text-indigo-600', 'hover:text-white'];
const activeCss = ['bg-indigo-500', 'text-white'];

export default function Toggle(props) {
  const activeApi = classNames('rounded-bl-lg', base, activeCss);
  const normalApi = classNames('rounded-bl-lg', base, normalCss);

  const activeDoc = classNames('rounded-tr-lg', 'border-l-none', base, activeCss);
  const normalDoc = classNames('rounded-tr-lg', 'border-l-none', base, normalCss);

  return (
    <ul className="flex justify-center mt-2">
      <li>
        <button className={props.active === 'api' ? activeApi : normalApi} onClick={() => props.showApi(true)}>
          API Demo
        </button>
      </li>
      <li>
        <button className={props.active === 'docs' ? activeDoc : normalDoc} onClick={() => props.showApi(false)}>
          More Documentation
        </button>
      </li>
    </ul>
  );
}
