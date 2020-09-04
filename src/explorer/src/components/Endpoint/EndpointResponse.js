import React from 'react';
// import { LightAsync as SyntaxHighlighter } from 'react-syntax-highlighter';
import SyntaxHighlighter from 'react-syntax-highlighter/dist/esm/light-async'
import json from 'react-syntax-highlighter/dist/esm/languages/hljs/javascript';
import theme from 'react-syntax-highlighter/dist/esm/styles/hljs/mono-blue';
import classNames from 'classnames';

SyntaxHighlighter.registerLanguage('json', json);

export default function Code({ success, code }) {
  const classes = classNames({
    'border-indigo-300': success,
    'border-red-300': !success
  }, 'max-w-3/4 break-all flex self-center mt-5 rounded border text-sm');

  return (
    <SyntaxHighlighter
      className={classes}
      language="json"
      style={theme}
      wrapLines={false}>
      {code}
    </SyntaxHighlighter>
  );
}
