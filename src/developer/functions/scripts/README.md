# Unclaimed key usage report

From `src/developer/functions`, install the existing workspace dependencies (`cd .. && pnpm install --frozen-lockfile`), then run:

```sh
export GOOGLE_APPLICATION_CREDENTIALS=/path/to/read-only-service-account.json
export REDIS_HOST=your-api-redis-host
node scripts/report-unclaimed-key-usage.mjs --months=6 --output=unclaimed-key-usage.csv
```

Application Default Credentials can also come from `gcloud auth application-default login`; ensure they select the intended Firestore project and can read `clients-unclaimed` and `keys`. The script does not write to Firestore or Redis. Set `REDIS_URL` instead of `REDIS_HOST` if using a URL. With `REDIS_HOST`, optional `REDIS_PORT` defaults to 6379; `REDIS_PASSWORD` and `REDIS_TLS=true` are supported. Use the same Redis instance as the API. Private production Redis requires approved VPN, tunnel, or network access; credentials alone cannot provide connectivity.

The CSV contains **API key values**; store and share it securely. `used_within_period` means Redis records a last request attempt at or after the UTC cutoff (six calendar months by default), not necessarily a successful request. `no_last_used_timestamp` means no timestamp is currently recorded; expiration, eviction, or data loss can erase evidence of historical use.
