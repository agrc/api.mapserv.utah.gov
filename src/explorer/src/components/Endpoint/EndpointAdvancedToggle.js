import React, { useState } from 'react';

export default function Toggle(props) {
  const [advanced, setAdvanced] = useState(false);

  return (
    <>
      <div className="flex items-center cursor-pointer pt-3 justify-between lg:justify-start" onClick={() => setAdvanced(!advanced)}>
        <h4 className="lg:w-1/5 -ml-3 text-gray-500 text-xl font-hairline tracking-tight">Advanced Usage</h4>
        <svg
          className="h-5 w-5 fill-current text-indigo-600 hover:text-indigo-500 transition duration-150 ease-in-out"
          viewBox="0 0 20 20"
          xmlns="http://www.w3.org/2000/svg">
          <path
            d="M17 16h2v-3h-6v3h2v4h2v-4zM1 9h6v3H1V9zm6-4h6v3H7V5zM3 0h2v8H3V0zm12 0h2v12h-2V0zM9 0h2v4H9V0zM3 12h2v8H3v-8zm6-4h2v12H9V8z"
            fillRule="evenodd"
          />
        </svg>
      </div>
      {advanced ? props.children : null}
    </>
  );
}
