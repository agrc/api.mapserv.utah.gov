module.exports = {
  empty: {
    result: [],
    status: 200,
  },
  attributeWithPredicate: {
    result: [
      {
        attributes: {
          name: "KANE",
        },
      },
    ],
    status: 200,
  },
  atlas: {
    county: { result: [{ attributes: { name: "WEBER" } }], status: 200 },
    nationalGrid: {
      result: [{ attributes: { grid1mil: "12T", grid100k: "VL" } }],
      status: 200,
    },
  },
};
