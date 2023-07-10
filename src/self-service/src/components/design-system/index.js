export function createKeyLookup(object) {
  return Object.keys(object).reduce((acc, key) => {
    acc[key] = key;

    return acc;
  }, {});
}
