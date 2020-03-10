import React from 'react';
import { LightAsync as SyntaxHighlighter } from 'react-syntax-highlighter';
import json from 'react-syntax-highlighter/dist/esm/languages/hljs/javascript';
import theme from 'react-syntax-highlighter/dist/esm/styles/hljs/mono-blue';

SyntaxHighlighter.registerLanguage('json', json);

export default function Code(props) {
  return (
    <SyntaxHighlighter
      className="max-w-3/4 break-all flex self-center mt-5 rounded border border-indigo-300 text-sm"
      language="json"
      style={theme}
      wrapLines={false}>
      {props.code}
    </SyntaxHighlighter>
  );
}
