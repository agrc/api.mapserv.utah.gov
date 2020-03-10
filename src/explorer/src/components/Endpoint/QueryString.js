import * as querystring from 'querystring';

export default function stringify(values, url, initial) {
  // clone values
  values = JSON.parse(JSON.stringify(values));

  const requiredParts = getRequiredParts(url);

  // replace required parts
  requiredParts.forEach(part => {
    url = url.replace(`:${part}`, values[part]);
    console.log(values);
    delete values[part];
  });

  for (const property in values) {
    // skip values that don't have an initial
    if (!initial.hasOwnProperty(property)) {
      continue;
    }

    // remove values that equal the initial
    if (values[property] === initial[property]) {
      delete values[property];
    }
  }

  const queryString = querystring.stringify(values);

  if (queryString) {
    return `${url}?${queryString}`;
  }

  return url;
};

const hasRequiredParts = (values, url, initial) => {
  const requiredParts = getRequiredParts(url);

  return requiredParts.every(key => {
    // are there required parts
    if (!values || !values.hasOwnProperty(key)) {
      return false;
    }

    // what is the value
    const value = values[key];

    // is it a good value
    if (value === null || value === undefined) {
      return false;
    }

    // is the value the same as the default
    return value !== initial[key];
  });
};

const getRequiredParts = (url) => {
  const requiredParts = [];
  const regex = /:(\w+)/g
  let match;

  while ((match = regex.exec(url))) {
    requiredParts.push(match[1]);
  }

  return requiredParts;
};


export { hasRequiredParts, getRequiredParts };
