/* eslint import/no-unresolved: off */
/* global console, process */
import { BigQuery } from '@google-cloud/bigquery';
import { applicationDefault, deleteApp, initializeApp } from 'firebase-admin/app';
import { getFirestore } from 'firebase-admin/firestore';
import { open } from 'node:fs/promises';
import { resolve } from 'node:path';
import { pathToFileURL } from 'node:url';

const headers = 'accountId,key,claimed,disabled,lastRequestUtc,lastSuccessUtc,requestsInPeriod,usageStatus,cutoffUtc';
const defaultProject = 'ut-dts-agrc-web-api-prod';
const analyticsTable = 'ugrc_api_analytics.ugrc_api_Middleware_RequestLoggerMiddleware';
const analyticsLocation = 'us-central1';

export const usageQuery = (project) => `
SELECT
  LOWER(jsonPayload.properties.key) AS key,
  MAX(timestamp) AS lastRequest,
  MAX(IF(jsonPayload.properties.result < 400, timestamp, NULL)) AS lastSuccess,
  COUNT(*) AS requests
FROM \`${project}.${analyticsTable}\`
WHERE timestamp >= @cutoff
  AND jsonPayload.properties.key IN UNNEST(@keys)
GROUP BY key`;

export const parseOptions = (args) => {
  const options = { months: '6', output: 'unclaimed-key-usage.csv', project: defaultProject };
  const allowed = ['--months', '--since', '--output', '--project'];
  const seen = new Set();

  for (let index = 0; index < args.length; index += 1) {
    const [name, inlineValue] = args[index].split('=', 2);

    if (!allowed.includes(name) || seen.has(name)) {
      throw new Error(`Unknown or duplicate option: ${name}`);
    }

    seen.add(name);
    const value = inlineValue ?? args[++index];

    if (!value || value.startsWith('--')) {
      throw new Error(`${name} requires a value.`);
    }

    options[name.slice(2)] = value;
  }

  if (seen.has('--months') && seen.has('--since')) {
    throw new Error('Use either --months or --since, not both.');
  }

  if (options.since) {
    if (!/^\d{4}-\d{2}-\d{2}$/.test(options.since) || Number.isNaN(Date.parse(`${options.since}T00:00:00Z`))) {
      throw new Error('--since must be a date formatted as YYYY-MM-DD.');
    }

    delete options.months;
  } else {
    options.months = Number(options.months);

    if (!Number.isSafeInteger(options.months) || options.months <= 0) {
      throw new Error('--months must be a positive whole number.');
    }
  }

  for (const name of ['output', 'project']) {
    if (!options[name].trim()) {
      throw new Error(`--${name} must be nonempty.`);
    }
  }

  return options;
};

export const cutoffForMonths = (months, now = new Date()) => {
  const cutoff = new Date(now);
  const day = cutoff.getUTCDate();
  cutoff.setUTCDate(1);
  cutoff.setUTCMonth(cutoff.getUTCMonth() - months);
  const lastDay = new Date(Date.UTC(cutoff.getUTCFullYear(), cutoff.getUTCMonth() + 1, 0)).getUTCDate();
  cutoff.setUTCDate(Math.min(day, lastDay));

  if (Number.isNaN(cutoff.getTime())) {
    throw new Error('--months produces a date outside the supported range.');
  }

  return cutoff;
};

export const cutoffForOptions = ({ months, since }, now = new Date()) =>
  since ? new Date(`${since}T00:00:00.000Z`) : cutoffForMonths(months, now);

const csvEscape = (value) => {
  let text = String(value ?? '');

  if (/^[\s]*[=+\-@]/.test(text)) {
    text = `'${text}`;
  }

  return /[",\r\n]/.test(text) ? `"${text.replaceAll('"', '""')}"` : text;
};

// BigQuery returns TIMESTAMP columns as BigQueryTimestamp objects with an ISO `value`
const isoFromTimestamp = (timestamp) => {
  if (!timestamp) {
    return '';
  }

  return new Date(timestamp.value ?? timestamp).toISOString();
};

export const reportRow = (apiKey, usage, cutoff) => {
  const requests = Number(usage?.requests ?? 0);
  const usageStatus = !usage ? 'no_requests' : usage.lastSuccess ? 'used_successfully' : 'rejected_requests_only';

  return [
    apiKey.accountId,
    apiKey.key,
    apiKey.claimed ?? false,
    apiKey.flags?.disabled ?? false,
    isoFromTimestamp(usage?.lastRequest),
    isoFromTimestamp(usage?.lastSuccess),
    requests,
    usageStatus,
    cutoff.toISOString(),
  ]
    .map(csvEscape)
    .join(',');
};

const batches = function* (values, size) {
  for (let index = 0; index < values.length; index += size) {
    yield values.slice(index, index + size);
  }
};

export const writeReport = async (outputPath, rows) => {
  const file = await open(outputPath, 'w', 0o600);

  try {
    await file.chmod(0o600);
    await file.writeFile(`${rows.join('\n')}\n`, 'utf8');
  } finally {
    await file.close();
  }
};

const main = async () => {
  const options = parseOptions(process.argv.slice(2));
  const cutoff = cutoffForOptions(options);
  const app = initializeApp({ credential: applicationDefault(), projectId: options.project });

  try {
    const db = getFirestore(app);
    const accounts = await db.collection('clients-unclaimed').get();
    const accountIds = accounts.docs.map((document) => document.id);
    const keys = [];

    for (const ids of batches(accountIds, 30)) {
      const snapshot = await db.collection('keys').where('accountId', 'in', ids).get();

      for (const document of snapshot.docs) {
        const key = document.data();

        if (key.flags?.deleted !== true) {
          keys.push({ ...key, key: key.key ?? document.id });
        }
      }
    }

    const usage = new Map();

    if (keys.length > 0) {
      const bigquery = new BigQuery({ projectId: options.project });
      const [rows] = await bigquery.query({
        query: usageQuery(options.project),
        params: { cutoff, keys: keys.map((key) => key.key) },
        types: { cutoff: 'TIMESTAMP', keys: ['STRING'] },
        location: analyticsLocation,
      });

      rows.forEach((row) => usage.set(row.key, row));
    }

    const rows = [headers, ...keys.map((key) => reportRow(key, usage.get(key.key.toLowerCase()), cutoff))];

    const outputPath = resolve(options.output);
    await writeReport(outputPath, rows);
    console.log(
      `Wrote ${keys.length} non-deleted key(s) from ${accountIds.length} unclaimed account(s) to ${outputPath}`,
    );
    console.log(`Cutoff: ${cutoff.toISOString()}`);
  } finally {
    await deleteApp(app);
  }
};

if (process.argv[1] && import.meta.url === pathToFileURL(resolve(process.argv[1])).href) {
  main().catch((error) => {
    console.error(`Report failed: ${error.message}`);
    process.exitCode = 1;
  });
}
