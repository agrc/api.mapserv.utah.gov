import React from 'react';
import CopyText from '@gradeup/copy-text-to-cb';

export default function Url(props) {
  return props.url ? (
    <div className="flex justify-center">
      <span className="max-w-3/4 break-all flex self-center p-2 text-base leading-6 text-gray-700 rounded-l border border-r-0 border-indigo-300 bg-gray-200">
        {props.url}
      </span>
      <CopyText
        text={props.url}
        classes="flex"
        pop={false}
        component={props => (
          <button
            {...props}
            className="cursor-pointer rounded-r border border-indigo-300 py-2 px-2 text-indigo-400 hover:text-white hover:bg-indigo-500 hover:border-indigo-600 focus:outline-none transition ease-in-out duration-150">
            Copy url
          </button>
        )}
      />
    </div>
  ) : null;
}
