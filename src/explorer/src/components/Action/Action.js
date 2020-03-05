import React from 'react';

export default function Action() {
  return (
    <div className="flex justify-center">
      <aside className="max-w-2xl w-full mt-8 flex sm:item-center justify-evenly flex-wrap bg-gray-100 p-5 rounded-md">
        <h2 className="text-2xl leading-7 font-extrabold tracking-tight sm:text-2xl sm:leading-6">
          Ready to dive in?
          <br />
          <span className="text-indigo-600">Get your API key today.</span>
        </h2>
        <div className="flex justify-center">
          <div className="flex mt-4 md:mt-0 rounded-md shadow">
            <a
              href="/get-started"
              className="flex items-center px-5 py-3 border border-transparent text-base leading-6 font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-500 focus:outline-none focus:shadow-outline transition duration-150 ease-in-out">
              Get started
            </a>
          </div>
        </div>
      </aside>
    </div>
  );
}
