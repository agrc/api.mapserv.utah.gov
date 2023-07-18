import { describe, expect, it } from 'vitest';
import { createKey as generateKey } from './keys';

describe.concurrent('keys', () => {
  it('is not empty', async () => {
    expect(generateKey()).not.toBeNull();
  });
  it('is unique', async () => {
    expect(generateKey()).not.toBe(generateKey());
  });
  it('is 19 characters', async () => {
    expect(generateKey().length).toBe(19);
  });
  it('starts with UGRC-', async () => {
    expect(generateKey().startsWith('UGRC-')).toBeTruthy();
  });
});
