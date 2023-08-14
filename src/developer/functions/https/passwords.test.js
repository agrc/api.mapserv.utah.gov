import { describe, expect, it } from 'vitest';
import { createKeyGen, hashPassword } from './passwords';

const salt = 'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx';
const pepper = 'yyyyyyyy';
const password = 'password123';
const dotnetHash =
  'DC07eaMXkaeK26KTIt3ldtzTdISTH5j9cmrg3OzFF16MvfKkEi9ihEkI8VDLzNvU';

describe('passwords', () => {
  it('generates the proper key', () => {
    const { key } = createKeyGen(password + salt, pepper);

    expect(Buffer.from(key).toString('base64')).toEqual(
      'TE/QSjrXPsCszWGNwcnguCzh99O5MY3THfu0AvEcfLo=',
    );
  });
  it('generates the proper iv', () => {
    const { iv } = createKeyGen(password + salt, pepper);

    expect(Buffer.from(iv).toString('base64')).toEqual(
      '1Rsly6J+7ptFukSfwBxXXA==',
    );
  });
  it('generates the proper hash', async () => {
    expect(hashPassword(password, salt, pepper)).toEqual(dotnetHash);
  });
});
