import { describe, expect, it } from 'vitest';
import { cutoffForMonths, dateFromTicks, parseOptions, reportRow } from './report-unclaimed-key-usage.mjs';

describe('unclaimed key usage report', () => {
  it('parses defaults and both option forms', () => {
    expect(parseOptions([])).toEqual({ months: 6, output: 'unclaimed-key-usage.csv' });
    expect(parseOptions(['--months=3', '--output', 'report.csv'])).toEqual({ months: 3, output: 'report.csv' });
  });

  it.each([
    ['--months=0'],
    ['--months=-2'],
    ['--months=1.5'],
    ['--months=abc'],
    ['--months'],
    ['--months', '--output=report.csv'],
    ['--output='],
    ['--unknown=1'],
    ['--months=2', '--months=3'],
  ])('rejects invalid arguments: %s', (...args) => {
    expect(() => parseOptions(args)).toThrow();
  });

  it('subtracts calendar months in UTC without rolling into the next month', () => {
    expect(cutoffForMonths(1, new Date('2026-03-31T15:02:03.000Z')).toISOString()).toBe('2026-02-28T15:02:03.000Z');
    expect(cutoffForMonths(6, new Date('2026-10-05T15:02:03.000Z')).toISOString()).toBe('2026-04-05T15:02:03.000Z');
  });

  it('converts .NET UTC ticks without losing precision to floating-point rounding', () => {
    expect(dateFromTicks('621355968000000000').toISOString()).toBe('1970-01-01T00:00:00.000Z');
    expect(dateFromTicks('638000000000120000').toISOString()).toBe('2022-09-28T22:13:20.012Z');
    expect(dateFromTicks(null)).toBeNull();
    expect(() => dateFromTicks('not ticks')).toThrow(/invalid/);
  });

  it('distinguishes recent, older and missing usage, and escapes CSV fields', () => {
    const key = { accountId: 'a,"b', key: '=danger', flags: { disabled: true } };
    const cutoff = new Date('2026-04-05T00:00:00.000Z');
    const ticks = (BigInt(cutoff.getTime()) * 10000n + 621355968000000000n).toString();

    expect(reportRow(key, ticks, cutoff)).toBe(
      '"a,""b",\'=danger,false,true,2026-04-05T00:00:00.000Z,true,used_within_period,2026-04-05T00:00:00.000Z',
    );
    expect(reportRow(key, (BigInt(ticks) - 10000n).toString(), cutoff)).toContain(',false,not_used_within_period,');
    expect(reportRow(key, null, cutoff)).toContain(',,false,no_last_used_timestamp,');
  });
});
