import { webcrypto } from 'node:crypto';
import { v4 as uuid } from 'uuid';

const min = 100000;
const max = 999999;

/**
 * A function that generates a new API key
 * It joins UGRC- with the first 8 characters of a UUID with a random number between 100000 and 999999
 * @returns {string}
 */
export const generateKey = () => {
  const key = uuid();
  let randomValue;

  do {
    randomValue = webcrypto.getRandomValues(new Uint32Array(1))[0];
  } while (randomValue < min || randomValue > max);

  return `ugrc-${key.substring(0, 8).toLowerCase()}${randomValue}`;
};
