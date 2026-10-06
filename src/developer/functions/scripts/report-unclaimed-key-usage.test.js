import { chmod, mkdtemp, readFile, rm, stat } from 'node:fs/promises';
import { tmpdir } from 'node:os';
import { join } from 'node:path';
import { describe, expect, it } from 'vitest';
import {
  cutoffForMonths,
  cutoffForOptions,
  parseOptions,
  reportRow,
  usageQuery,
  writeReport,
} from './report-unclaimed-key-usage.mjs';

describe('unclaimed key usage report', () => {
  it('parses defaults and both option forms', () => {
    expect(parseOptions([])).toEqual({
      months: 6,
      output: 'unclaimed-key-usage.csv',
      project: 'ut-dts-agrc-web-api-prod',
    });
    expect(parseOptions(['--months=3', '--output', 'report.csv', '--project=dev'])).toEqual({
      months: 3,
      output: 'report.csv',
      project: 'dev',
    });
    expect(parseOptions(['--since=2024-08-05'])).toEqual({
      since: '2024-08-05',
      output: 'unclaimed-key-usage.csv',
      project: 'ut-dts-agrc-web-api-prod',
    });
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
    ['--months=2', '--since=2024-08-05'],
    ['--since=08/05/2024'],
    ['--since=2024-13-01'],
    ['--project='],
  ])('rejects invalid arguments: %s', (...args) => {
    expect(() => parseOptions(args)).toThrow();
  });

  it('subtracts calendar months in UTC without rolling into the next month', () => {
    expect(cutoffForMonths(1, new Date('2026-03-31T15:02:03.000Z')).toISOString()).toBe('2026-02-28T15:02:03.000Z');
    expect(cutoffForMonths(6, new Date('2026-10-05T15:02:03.000Z')).toISOString()).toBe('2026-04-05T15:02:03.000Z');
  });

  it('uses a UTC midnight cutoff for --since', () => {
    expect(cutoffForOptions({ since: '2024-08-05' }).toISOString()).toBe('2024-08-05T00:00:00.000Z');
    expect(cutoffForOptions({ months: 1 }, new Date('2026-03-31T15:02:03.000Z')).toISOString()).toBe(
      '2026-02-28T15:02:03.000Z',
    );
  });

  it('queries the request logger table with parameters, never interpolated keys', () => {
    const query = usageQuery('ut-dts-agrc-web-api-prod');

    expect(query).toContain(
      '`ut-dts-agrc-web-api-prod.ugrc_api_analytics.ugrc_api_Middleware_RequestLoggerMiddleware`',
    );
    expect(query).toContain('timestamp >= @cutoff');
    expect(query).toContain('IN UNNEST(@keys)');
  });

  it('distinguishes successful, rejected-only and missing usage, and escapes CSV fields', () => {
    const key = { accountId: 'a,"b', key: '=danger', flags: { disabled: true } };
    const cutoff = new Date('2026-04-05T00:00:00.000Z');
    const lastRequest = { value: '2026-05-01T12:00:00.000Z' };
    const lastSuccess = { value: '2026-04-30T12:00:00.000Z' };

    expect(reportRow(key, { lastRequest, lastSuccess, requests: 12 }, cutoff)).toBe(
      '"a,""b",\'=danger,false,true,2026-05-01T12:00:00.000Z,2026-04-30T12:00:00.000Z,12,used_successfully,2026-04-05T00:00:00.000Z',
    );
    expect(reportRow(key, { lastRequest, lastSuccess: null, requests: 3 }, cutoff)).toContain(
      ',2026-05-01T12:00:00.000Z,,3,rejected_requests_only,',
    );
    expect(reportRow(key, undefined, cutoff)).toContain(',,,0,no_requests,');
  });

  it('restricts access to new and existing reports', async () => {
    const directory = await mkdtemp(join(tmpdir(), 'unclaimed-key-report-'));
    const output = join(directory, 'report.csv');

    try {
      await writeReport(output, ['key', 'sensitive']);
      expect((await stat(output)).mode & 0o777).toBe(0o600);

      await chmod(output, 0o644);
      await writeReport(output, ['key', 'updated']);
      expect((await stat(output)).mode & 0o777).toBe(0o600);
      expect(await readFile(output, 'utf8')).toBe('key\nupdated\n');
    } finally {
      await rm(directory, { recursive: true, force: true });
    }
  });
});
