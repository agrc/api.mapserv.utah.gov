# Unclaimed key usage report

Lists every key belonging to an unclaimed account (`clients-unclaimed`), skipping keys flagged as deleted or disabled, along with its Firestore details (created, elevated, production, server, machine name, notes, pattern and regular expression) and its request history from the API's BigQuery request log (`ugrc_api_analytics.ugrc_api_Middleware_RequestLoggerMiddleware`).

From `src/developer/functions`, install the existing workspace dependencies (`cd .. && pnpm install --frozen-lockfile`), then run:

```sh
gcloud auth application-default login
node scripts/report-unclaimed-key-usage.mjs --months=6 --output=unclaimed-key-usage.csv
```

Options:

- `--months=<n>` looks back `n` calendar months (default `6`).
- `--since=<YYYY-MM-DD>` looks back to a UTC date instead, e.g. `--since=2024-08-05` for all usage since the id.utah.gov migration. Request logging began on 2024-08-07.
- `--project=<id>` sets the Google Cloud project for both Firestore and BigQuery (default `ut-dts-agrc-web-api-prod`).
- `--output=<path>` sets the CSV path (default `unclaimed-key-usage.csv`).

Your credentials need read access to Firestore (`clients-unclaimed` and `keys`) and permission to run BigQuery jobs and read the analytics dataset (`roles/bigquery.jobUser` and `roles/bigquery.dataViewer`). The script does not write to Firestore or BigQuery. A six-month run scans about 1.5 GB in BigQuery; the full history scans about 6 GB.

The CSV contains **API key values** and is written with owner-only permissions; store and share it securely. `usageStatus` is one of:

- `used_successfully`: at least one request since the cutoff returned a status below 400.
- `rejected_requests_only`: the key made requests since the cutoff, but all of them failed (e.g. disabled key or bad referrer).
- `no_requests`: no requests with this key since the cutoff.
