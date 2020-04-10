import React from 'react';

export function Item(props) {
  return (
    <div className="flex flex-col">
      <div className="justify-center self-center">
        <svg className="fill-current text-indigo-600" height="50" width="50" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
          <defs />
          <path d="M0 11l2-2 5 5L18 3l2 2L7 18z" fill-rule="evenodd" />
        </svg>
      </div>
      <div className="uppercase text-center justify-center self-center text-xl font-light">{props.children}</div>
    </div>
  );
}
