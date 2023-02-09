import React from 'react';

export function Item(props) {
  return (
    <div className="flex flex-col">
      <div className="justify-center self-center">
        <svg className="mb-3 fill-current text-indigo-600" height="30" width="30" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
          <defs/>
          <path d="M11 0L9 2v6H2l-2 2v2l2 6 3 2h8l2-2v-8l-3-7V0h-1zm6 10h3v10h-3V10z" fill="current" fillRule="evenodd"/>
        </svg>
      </div>
      <div className="tracking-widest uppercase text-center justify-center self-center text-base font-thin">{props.children}</div>
    </div>
  );
}
