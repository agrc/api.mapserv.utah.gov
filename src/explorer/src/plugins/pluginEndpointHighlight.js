// @ts-check
import { definePlugin, InlineStyleAnnotation } from '@expressive-code/core';

const versionRegex = /(?<version>^.*\/v1\/)/g;
const pathRegex = /(?<path>[a-z]+)[\/|\?]/gi;
const parameterRegex = /\{(?<parameter>[^}]+)\}\/?/g;
const skipTokens = ['gov', 'api'];

export const getVersion = (url) => {
  const version = versionRegex.exec(url)?.groups?.version ?? '';

  return {
    version: version,
    start: 0,
    end: version.length,
  };
};
export const getPaths = (url) => {
  const paths = [];

  let match;
  while ((match = pathRegex.exec(url)) !== null) {
    let path = match.groups?.path ?? '';
    if (skipTokens.includes(path)) {
      continue;
    }

    paths.push({
      path,
      start: match.index,
      end: match.index + path.length,
    });
  }

  return paths;
};
export const getParameters = (url) => {
  const parameters = [];
  url = url.split('?')[0];

  if (!url) {
    return parameters;
  }

  let match;
  while ((match = parameterRegex.exec(url)) !== null) {
    let parameter = match.groups?.parameter ?? '';

    let start = match.index;
    parameters.push({
      parameter,
      start,
      end: start + parameter.length + 2,
    });
  }

  return parameters;
};
export const getOptionalParameters = (url) => {
  const parameters = [];
  const [base, queryString] = url.split('?');

  if (!queryString) {
    return parameters;
  }

  let match;
  while ((match = parameterRegex.exec(queryString)) !== null) {
    let parameter = match.groups?.parameter ?? '';

    let start = base.length + match.index + 1;
    parameters.push({
      parameter,
      start,
      end: start + parameter.length + 2,
    });
  }

  return parameters;
};

const dark = 0;
const light = 1;
export function pluginEndpointHighlight() {
  return definePlugin({
    name: 'Endpoint Highlight',
    hooks: {
      preprocessCode: (context) => {
        // Only apply this to code blocks with the `endpoint` meta
        if (!context.codeBlock.meta.includes('endpoint')) {
          return;
        }

        console.log('language', context.codeBlock.language);
        console.log('meta', context.codeBlock.meta);

        const line = context.codeBlock.getLine(0);

        if (!line?.text) {
          return;
        }

        console.log('code block', line.text);

        const version = getVersion(line.text);
        const paths = getPaths(line.text);
        const parameters = getParameters(line.text);
        const options = getOptionalParameters(line.text);

        console.log('adding bold annotation', version.start, version.end);
        line.addAnnotation(
          new InlineStyleAnnotation({
            inlineRange: {
              columnStart: version.start,
              columnEnd: version.end,
            },
            color: '#D6DEEB',
            styleVariantIndex: dark,
          }),
        );
        line.addAnnotation(
          new InlineStyleAnnotation({
            inlineRange: {
              columnStart: version.start,
              columnEnd: version.end,
            },
            color: '#403F53',
            styleVariantIndex: light,
          }),
        );

        paths.forEach((data) => {
          console.log('adding color annotation', data.start, data.end);

          line.addAnnotation(
            new InlineStyleAnnotation({
              inlineRange: {
                columnStart: data.start,
                columnEnd: data.end,
              },
              color: '#7FDBCA',
              bold: true,
              styleVariantIndex: dark,
            }),
          );

          line.addAnnotation(
            new InlineStyleAnnotation({
              inlineRange: {
                columnStart: data.start,
                columnEnd: data.end,
              },
              color: '#097174',
              bold: true,
              styleVariantIndex: light,
            }),
          );
        });

        parameters.forEach((data) => {
          console.log('adding color annotation', data.start, data.end);
          line.addAnnotation(
            new InlineStyleAnnotation({
              inlineRange: {
                columnStart: data.start,
                columnEnd: data.end,
              },
              color: '#C789D6',
              bold: true,
              styleVariantIndex: dark,
            }),
          );
          line.addAnnotation(
            new InlineStyleAnnotation({
              inlineRange: {
                columnStart: data.start,
                columnEnd: data.end,
              },
              color: '#7F5889',
              bold: true,
              styleVariantIndex: light,
            }),
          );
        });

        options.forEach((data) => {
          console.log('adding color annotation', data.start, data.end);
          line.addAnnotation(
            new InlineStyleAnnotation({
              inlineRange: {
                columnStart: data.start,
                columnEnd: data.end,
              },
              color: '#F78C6C',
              bold: true,
              styleVariantIndex: dark,
            }),
          );
          line.addAnnotation(
            new InlineStyleAnnotation({
              inlineRange: {
                columnStart: data.start,
                columnEnd: data.end,
              },
              color: '#AA0982',
              bold: true,
              styleVariantIndex: light,
            }),
          );
        });
      },
    },
  });
}
