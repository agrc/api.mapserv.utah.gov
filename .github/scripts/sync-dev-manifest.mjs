// Raises each version in the dev release-please manifest to the version main
// has released when main's is newer, e.g. dev 1.17.13-rc.2 + main 1.17.13 ->
// 1.17.13, so the next dev pre-release becomes 1.17.14-rc.0.
//
// usage: node sync-dev-manifest.mjs <main manifest> <dev manifest>
import { readFileSync, writeFileSync } from "node:fs";

const [mainPath, devPath] = process.argv.slice(2);
const main = JSON.parse(readFileSync(mainPath, "utf8"));
const dev = JSON.parse(readFileSync(devPath, "utf8"));

const parse = (version) => {
  const [core, pre] = version.split("-", 2);

  return { core: core.split(".").map(Number), pre };
};

const compareIdentifiers = (a, b) => {
  const aIsNumber = /^\d+$/.test(a);
  const bIsNumber = /^\d+$/.test(b);

  if (aIsNumber && bIsNumber) {
    return Number(a) - Number(b);
  }

  if (aIsNumber !== bIsNumber) {
    return aIsNumber ? -1 : 1;
  }

  return a < b ? -1 : a > b ? 1 : 0;
};

// semver precedence: core numbers, then a release outranks any pre-release
const compare = (a, b) => {
  const left = parse(a);
  const right = parse(b);

  for (let i = 0; i < 3; i++) {
    if (left.core[i] !== right.core[i]) {
      return left.core[i] - right.core[i];
    }
  }

  if (!left.pre || !right.pre) {
    return (left.pre ? -1 : 0) - (right.pre ? -1 : 0);
  }

  const leftIds = left.pre.split(".");
  const rightIds = right.pre.split(".");

  for (let i = 0; i < Math.min(leftIds.length, rightIds.length); i++) {
    const result = compareIdentifiers(leftIds[i], rightIds[i]);

    if (result !== 0) {
      return result;
    }
  }

  return leftIds.length - rightIds.length;
};

for (const [path, version] of Object.entries(main)) {
  if (!dev[path] || compare(version, dev[path]) > 0) {
    console.log(`${path}: ${dev[path] ?? "(none)"} -> ${version}`);
    dev[path] = version;
  }
}

writeFileSync(devPath, `${JSON.stringify(dev, null, 2)}\n`);
