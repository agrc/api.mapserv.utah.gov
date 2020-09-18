import React, { useState } from 'react';
import EndpointDemoDocToggle from './EndpointDemoDocToggle';
import EndpointResponse from './EndpointResponse';
import EndpointUrl from './EndpointUrl';
import Button from '../Button';
import { Tip } from '../Endpoint/Documentation/Elements';

const getComponent = (key, children) => {
  return children.filter(comp => {
    return comp.key === key;
  });
};

export default function Endpoint(props) {
  let defaultValue = true;
  if (props.collapsed !== undefined) {
    defaultValue = props.collapsed;
  }

  const fetchApi = async url => {
    if (!url) {
      return;
    }

    try {
      const response = await fetch(url, {
        method: 'GET',
        mode: 'cors'
      });

      const result = await response.json();

      setResponse({ success: true, code: JSON.stringify(result, null, 2) });
    } catch (error) {
      setResponse({ success: false, code: error.message });
    }
  };

  const [collapsed, setCollapsed] = useState(defaultValue);
  const [api, setApi] = useState(true);
  const [response, setResponse] = useState();

  return (
    <article className="bg-white shadow m-3 border-b border-gray-200 rounded-lg">
      <header onClick={() => setCollapsed(!collapsed)} className="p-3 flex justify-between items-center w-full cursor-pointer">
        <div>
          <h3 className="text-2xl font-hairline tracking-tight">{props.name}</h3>
          <p className="text-sm text-gray-500 tracking-wider">{props.description}</p>
        </div>
        <div className="flex justify-end">
          <div className="flex mt-4 md:mt-0">
            <button className="flex cursor-pointer focus:outline-none px-10 py-3">
              {collapsed ? (
                <svg
                  className="h-10 w-10 fill-current text-indigo-600 hover:text-indigo-500 transition duration-150 ease-in-out"
                  viewBox="0 0 20 20"
                  xmlns="http://www.w3.org/2000/svg">
                  <path
                    d="M9.293 12.536L5.757 9l1.415-1.414L10 10.414l2.828-2.828L14.243 9 10 13.243l-.707-.707zM20 10c0-5.523-4.477-10-10-10S0 4.477 0 10s4.477 10 10 10 10-4.477 10-10zM10 2a8 8 0 100 16 8 8 0 000-16z"
                    fillRule="evenodd"
                  />
                </svg>
              ) : (
                <svg
                  className="h-10 w-10 fill-current text-indigo-600 hover:text-indigo-500 transition duration-150 ease-in-out"
                  viewBox="0 0 20 20"
                  xmlns="http://www.w3.org/2000/svg">
                  <path
                    d="M11.414 10l2.829-2.828-1.415-1.415L10 8.586 7.172 5.757 5.757 7.172 8.586 10l-2.829 2.828 1.415 1.415L10 11.414l2.828 2.829 1.415-1.415L11.414 10zM2.93 17.071c3.905 3.905 10.237 3.905 14.142 0 3.905-3.905 3.905-10.237 0-14.142-3.905-3.905-10.237-3.905-14.142 0-3.905 3.905-3.905 10.237 0 14.142zm1.414-1.414A8 8 0 1015.657 4.343 8 8 0 004.343 15.657z"
                    fillRule="evenodd"
                  />
                </svg>
              )}
            </button>
          </div>
        </div>
      </header>
      {collapsed ? null : (
        <form
          onSubmit={event => {
            event.preventDefault();
            fetchApi(props.fetchUrl);
          }}
          className="relative bg-gray-100 border-t border-gray-200 rounded-b sm:rounded-b-lg">
          <EndpointDemoDocToggle active={api ? 'api' : 'docs'} showApi={setApi}></EndpointDemoDocToggle>
          <section className="flex">
            <section className="w-full">{getComponent(api ? 'api' : 'docs', props.children)}</section>
          </section>
          {api ? (
            <div className="flex flex-col justify-center w-full py-3">
              <EndpointUrl url={props.displayUrl}></EndpointUrl>
              {props.invalidCharacter ?
                <Tip className="mt-3">
                  Just a heads up! We have automatically encoded "{props.invalidCharacter}" in your street value. You'll want to do this in your code.
                </Tip> : null}
              {response ? <EndpointResponse {...response}></EndpointResponse> : null}
              <Button type="submit" disabled={!props.fetchUrl || props.fetchUrl.length < 1} className="justify-center w-1/2 self-center my-5 font-medium">
                Send it
              </Button>
            </div>
          ) : null}
        </form>
      )}
    </article>
  );
}
