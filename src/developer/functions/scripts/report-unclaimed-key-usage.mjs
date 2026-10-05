/* eslint import/no-unresolved: off */
/* global console, process */
import { applicationDefault, deleteApp, initializeApp } from 'firebase-admin/app';
import { getFirestore } from 'firebase-admin/firestore';
import { Redis } from 'ioredis';
import { writeFile } from 'node:fs/promises';
import { resolve } from 'node:path';
import { pathToFileURL } from 'node:url';

const headers = 'accountId,key,claimed,disabled,lastUsedUtc,usedWithinPeriod,usageStatus,cutoffUtc';
const dotNetEpochTicks = 621355968000000000n;
const ticksPerMillisecond = 10000n;

export const parseOptions = (args) => {
  const options = { months: 6, output: 'unclaimed-key-usage.csv' };
  const seen = new Set();

  for (let index = 0; index < args.length; index += 1) {
    const [name, inlineValue] = args[index].split('=', 2);

    if (!['--months', '--output'].includes(name) || seen.has(name)) {
      throw new Error(`Unknown or duplicate option: ${name}`);
    }

    seen.add(name);
    const value = inlineValue ?? args[++index];

    if (!value || value.startsWith('--')) {
      throw new Error(`${name} requires a value.`);
    }

    options[name.slice(2)] = value;
  }

  options.months = Number(options.months);

  if (!Number.isSafeInteger(options.months) || options.months <= 0) {
    throw new Error('--months must be a positive whole number.');
  }

  if (!options.output.trim()) {
    throw new Error('--output must be a nonempty path.');
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

export const dateFromTicks = (value) => {
  if (value === null) {
    return null;
  }

  if (!/^\d+$/.test(value)) {
    throw new Error('Redis returned an invalid .NET last-used timestamp.');
  }

  const difference = BigInt(value) - dotNetEpochTicks;
  let milliseconds = difference / ticksPerMillisecond;

  if (difference < 0n && difference % ticksPerMillisecond !== 0n) {
    milliseconds -= 1n;
  }

  const date = new Date(Number(milliseconds));

  if (Number.isNaN(date.getTime())) {
    throw new Error('Redis returned an out-of-range .NET last-used timestamp.');
  }

  return date;
};

const csvEscape = (value) => {
  let text = String(value ?? '');

  if (/^[\s]*[=+\-@]/.test(text)) {
    text = `'${text}`;
  }

  return /[",\r\n]/.test(text) ? `"${text.replaceAll('"', '""')}"` : text;
};

export const reportRow = (apiKey, timestamp, cutoff) => {
  const lastUsed = dateFromTicks(timestamp);
  const usedWithinPeriod = lastUsed !== null && lastUsed >= cutoff;
  const usageStatus =
    lastUsed === null ? 'no_last_used_timestamp' : usedWithinPeriod ? 'used_within_period' : 'not_used_within_period';

  return [
    apiKey.accountId,
    apiKey.key,
    apiKey.claimed ?? false,
    apiKey.flags?.disabled ?? false,
    lastUsed?.toISOString() ?? '',
    usedWithinPeriod,
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

const redisConfig = (env) => {
  if (!env.REDIS_URL && !env.REDIS_HOST) {
    throw new Error('Set REDIS_URL or REDIS_HOST to the API Redis endpoint before running this report.');
  }

  const tls = env.REDIS_TLS === 'true' ? {} : undefined;

  if (env.REDIS_URL) {
    return [env.REDIS_URL, { tls, lazyConnect: true }];
  }

  const port = Number(env.REDIS_PORT ?? 6379);

  if (!Number.isInteger(port) || port < 1 || port > 65535) {
    throw new Error('REDIS_PORT must be an integer from 1 to 65535.');
  }

  return [{ host: env.REDIS_HOST, port, password: env.REDIS_PASSWORD || undefined, tls, lazyConnect: true }];
};

const main = async () => {
  const { months, output } = parseOptions(process.argv.slice(2));
  const cutoff = cutoffForMonths(months);
  const connection = redisConfig(process.env);
  const app = initializeApp({ credential: applicationDefault() });
  const redis = new Redis(...connection);

  try {
    const db = getFirestore(app);
    await redis.connect();
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

    const rows = [headers];

    for (const keyBatch of batches(keys, 500)) {
      const timestamps = await redis.mget(...keyBatch.map((key) => `analytics:time:${key.key.toLowerCase()}`));

      keyBatch.forEach((key, index) => rows.push(reportRow(key, timestamps[index], cutoff)));
    }

    const outputPath = resolve(output);
    await writeFile(outputPath, `${rows.join('\n')}\n`, 'utf8');
    console.log(
      `Wrote ${keys.length} non-deleted key(s) from ${accountIds.length} unclaimed account(s) to ${outputPath}`,
    );
    console.log(`Cutoff: ${cutoff.toISOString()}`);
  } finally {
    redis.disconnect();
    await deleteApp(app);
  }
};

if (process.argv[1] && import.meta.url === pathToFileURL(resolve(process.argv[1])).href) {
  main().catch((error) => {
    console.error(`Report failed: ${error.message}`);
    process.exitCode = 1;
  });
}
