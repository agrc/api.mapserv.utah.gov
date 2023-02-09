import React from 'react';


const EndpointCategory = ({ id, title, subtitle, children }) => {
  return (
    <>
      <div
        id={id}
        className="m-3 bg-white border-l-2 border-indigo-300 md:rounded-l md:rounded-r border-r-8 border-t-2 border-b-2">
        <header className="p-3">
          <h3 className="text-3xl font-extrabold tracking-tight">{title}</h3>
          <p className="text-sm text-gray-500 tracking-wider">{subtitle}</p>
        </header>
      </div>
      {React.Children.map(children, child => React.cloneElement(child, { category: id }))}
    </>
  );
};

export default EndpointCategory;
