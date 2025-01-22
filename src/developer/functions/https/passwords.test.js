import { describe, expect, it } from 'vitest';
import { createKeyGen, encryptPassword } from './passwords';

const salt = 'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx';
const pepper = 'yyyyyyyy';
const password = 'password123';
const dotnetHash = `\f-;y�\u0017���ۢ�\"��v��t��\u001f��rj����\u0017^���\u0012/b�I\b�P����`; // eslint-disable-line

describe('passwords', () => {
  it('generates the proper key', () => {
    const { key } = createKeyGen(password + salt, pepper);

    expect(Buffer.from(key).toString('base64')).toEqual('TE/QSjrXPsCszWGNwcnguCzh99O5MY3THfu0AvEcfLo=');
  });
  it('generates the proper iv', () => {
    const { iv } = createKeyGen(password + salt, pepper);

    expect(Buffer.from(iv).toString('base64')).toEqual('1Rsly6J+7ptFukSfwBxXXA==');
  });
  it('generates the proper hash', async () => {
    expect(encryptPassword(password, salt, pepper)).toEqual(dotnetHash);
  });
});
