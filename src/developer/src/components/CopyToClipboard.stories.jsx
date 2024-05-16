import CopyToClipboard from './CopyToClipboard';

export default {
  title: 'Self service/CopyToClipboard',
  component: CopyToClipboard,
};

export const Default = () => <CopyToClipboard text="*.example.com" />;
