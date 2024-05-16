import { CheckIcon, ClipboardIcon } from '@heroicons/react/24/outline';
import PropTypes from 'prop-types';
import { useEffect, useState } from 'react';
import { CopyToClipboard as CopyToClipboard_lib } from 'react-copy-to-clipboard';
import { twMerge } from 'tailwind-merge';

export default function CopyToClipboard({ text, className }) {
  const [copied, setCopied] = useState(false);

  useEffect(() => {
    if (copied) {
      const timeout = setTimeout(() => {
        setCopied(false);
      }, 2000);

      return () => clearTimeout(timeout);
    }
  }, [copied]);

  return (
    <CopyToClipboard_lib
      text={text}
      className={twMerge(
        className,
        'h-8 cursor-pointer hover:text-wavy-400 dark:hover:text-mustard-500',
      )}
      onCopy={() => setCopied(true)}
    >
      {copied ? (
        <CheckIcon title="copied" />
      ) : (
        <ClipboardIcon title="copy to clipboard" />
      )}
    </CopyToClipboard_lib>
  );
}

CopyToClipboard.propTypes = {
  text: PropTypes.string.isRequired,
  className: PropTypes.string,
};
