import React from 'react';
import { useParams } from 'react-router-dom';


const EndpointCategory = ({ id, title, subtitle }) => {
  const { endpointCategory } = useParams();
  const nodeRef = React.useRef();

  React.useEffect(() => {
    if (endpointCategory === id) {
      nodeRef.current.scrollIntoView(true);
    }
  }, [endpointCategory, id]);

  return (
    <div
      ref={nodeRef}
      id={id}
      className="m-3 bg-white border-l-2 border-indigo-300 md:rounded-l md:rounded-r border-r-8 border-t-2 border-b-2">
      <header className="p-3">
        <h3 className="text-3xl font-extrabold tracking-tight">{title}</h3>
        <p className="text-sm text-gray-500 tracking-wider">{subtitle}</p>
      </header>
    </div>
  );
};

export default EndpointCategory;
