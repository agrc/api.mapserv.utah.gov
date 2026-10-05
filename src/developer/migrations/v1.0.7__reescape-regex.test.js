import { describe, expect, it, vi } from 'vitest';
import { migrate } from './v1.0.7__reescape-regex.cjs';

const createFirestore = (keys) => {
  const update = vi.fn();
  const commit = vi.fn().mockResolvedValue(undefined);

  const firestore = {
    collectionGroup: () => ({
      get: async () => ({
        docs: keys.map((data, index) => ({ ref: `keys/${index}`, data: () => data })),
      }),
    }),
    batch: () => ({ update, commit }),
  };

  return { firestore, update, commit };
};

describe('v1.0.7 reescape regex', () => {
  it('regenerates the regular expression for browser keys from their pattern', async () => {
    const { firestore, update, commit } = createFirestore([
      { pattern: 'example.com', regularExpression: '^https?:\\/\\/example\\.com', flags: { server: false } },
      {
        pattern: '*.example.com/*',
        regularExpression: '^https?:\\/\\/.+\\.example\\.com/.*',
        flags: { server: false },
      },
    ]);

    await migrate({ firestore });

    expect(update).toHaveBeenCalledTimes(2);
    expect(update).toHaveBeenNthCalledWith(1, 'keys/0', {
      regularExpression: '^https?:\\/\\/example\\.com(?::\\d+)?(?:[/?#]|$)',
    });
    expect(update).toHaveBeenNthCalledWith(2, 'keys/1', {
      regularExpression: '^https?:\\/\\/[a-z0-9._-]+\\.example\\.com\\/',
    });
    expect(commit).toHaveBeenCalledTimes(1);
  });
  it('disables browser keys whose pattern can not be expressed safely', async () => {
    const { firestore, update } = createFirestore([
      { pattern: '(a+)+$', regularExpression: '^https?:\\/\\/(a+)+$', flags: { server: false } },
      { pattern: '', regularExpression: '', flags: { server: false } },
      { flags: { server: false } },
    ]);

    await migrate({ firestore });

    expect(update).toHaveBeenCalledTimes(3);
    for (const index of [0, 1, 2]) {
      expect(update).toHaveBeenNthCalledWith(index + 1, `keys/${index}`, {
        regularExpression: '',
        'flags.disabled': true,
      });
    }
  });
  it('skips server keys', async () => {
    const { firestore, update, commit } = createFirestore([
      { pattern: '0.0.0.0', regularExpression: '', flags: { server: true } },
      { pattern: 'example.com' },
    ]);

    await migrate({ firestore });

    expect(update).not.toHaveBeenCalled();
    expect(commit).toHaveBeenCalledTimes(1);
  });
});
