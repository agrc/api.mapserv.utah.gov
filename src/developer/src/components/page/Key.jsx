import { useParams } from 'react-router-dom';

const Component = () => {
  const { key } = useParams();

  return <div>{key}</div>;
};
Component.displayName = 'Key';

export default Component;
