import React from 'react';

export default function Action() {
  return (
    <div className="flex justify-center my-4">
      <aside className="max-w-xl w-full my-8 flex sm:item-center justify-evenly flex-wrap bg-white p-5 rounded-lg border border-gray-300">
        <h2 className="text-2xl leading-7 font-extrabold tracking-tight sm:text-2xl sm:leading-6">
          Ready to dive in?
          <br />
          <span className="text-indigo-600">Get your API key today.</span>
        </h2>
        <div className="flex justify-center">
          <div className="flex mt-4 md:mt-0 rounded-md shadow">
            <a
              href="https://developer.mapserv.utah.gov"
              className="flex items-center px-5 py-3 border border-transparent text-base leading-6 font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-500 focus:outline-none focus:shadow-outline transition duration-150 ease-in-out">
              Get started
            </a>
          </div>
        </div>
      </aside>
    </div>
  );
}
