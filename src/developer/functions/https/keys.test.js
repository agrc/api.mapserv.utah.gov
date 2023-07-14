import { describe, expect, it } from 'vitest';
import { createKey } from './keys';

describe.concurrent('keys', () => {
  it('is not empty', async () => {
    expect(createKey()).not.toBeNull();
  });
  it('is unique', async () => {
    expect(createKey()).not.toBe(createKey());
  });
  it('is 19 characters', async () => {
    expect(createKey().length).toBe(19);
  });
  it('starts with UGRC-', async () => {
    expect(createKey().startsWith('UGRC-')).toBeTruthy();
  });
});
