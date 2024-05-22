import assert from "node:assert";
import test from "node:test";
import {
  getOptionalParameters,
  getParameters,
  getPaths,
  getVersion,
} from "./pluginEndpointHighlight.js";
const url =
  "https://api.mapserv.utah.gov/api/v1/geocode/milepost/{route}/{milepost}?apikey={apikey}";
const url2 =
  "https://api.mapserv.utah.gov/api/v1/info/featureClassNames?apikey={apikey}";

test("get version", () => {
  const { version, end, start } = getVersion(url);

  assert.strictEqual(version, "https://api.mapserv.utah.gov/api/v1/");
  assert.strictEqual(start, 0);
  assert.strictEqual(end, 36);
});

test("get endpoint categories", () => {
  const paths = getPaths(url);

  assert.deepStrictEqual(paths, [
    { path: "geocode", start: 36, end: 43 },
    { path: "milepost", start: 44, end: 52 },
  ]);

  const paths2 = getPaths(url2);
  assert.deepStrictEqual(paths2, [
    { path: "info", start: 36, end: 40 },
    { path: "featureClassNames", start: 41, end: 58 },
  ]);
});

test("get required parameters", () => {
  const parameters = getParameters(url);

  assert.deepStrictEqual(parameters, [
    { parameter: "route", start: 53, end: 60 },
    { parameter: "milepost", start: 61, end: 71 },
  ]);
});

test("get optional parameters", () => {
  const parameters = getOptionalParameters(url);

  assert.deepStrictEqual(parameters, [
    {
      parameter: "apikey",
      start: 79,
      end: 87,
    },
  ]);
});
