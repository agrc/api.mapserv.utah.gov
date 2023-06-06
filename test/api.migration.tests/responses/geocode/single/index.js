module.exports = {
  single: {
    result: {
      location: {
        x: 425608.67349758215,
        y: 4513499.5088544935,
      },
      score: 98.24,
      locator: "AddressPoints.AddressGrid",
      matchAddress: "326 E SOUTH TEMPLE ST, SALT LAKE CITY",
      inputAddress: "326 east south temple, slc",
      addressGrid: "SALT LAKE CITY",
    },
    status: 200,
  },
  single404: {
    status: 404,
    message: "No address candidates found with a score of 70 or better.",
  },
  singleEsriJson: {
    result: {
      attributes: {
        Location: { x: 425608.67349758215, y: 4513499.5088544935 },
        Score: 98.24,
        Locator: "AddressPoints.AddressGrid",
        MatchAddress: "326 E SOUTH TEMPLE ST, SALT LAKE CITY",
        InputAddress: "326 east south temple, slc",
        AddressGrid: "SALT LAKE CITY",
        ScoreDifference: -1.0,
        Wkid: 26912,
        Candidates: [],
      },
      geometry: {
        x: 425608.67349758215,
        y: 4513499.5088544935,
        type: "point",
        spatialReference: { wkid: 26912 },
      },
    },
    status: 200,
  },
  singleGeoJson: {
    result: {
      type: "Feature",
      geometry: {
        coordinates: [425608.67349758215, 4513499.5088544935],
        type: "Point",
      },
      properties: {
        Location: { x: 425608.67349758215, y: 4513499.5088544935 },
        Score: 98.24,
        Locator: "AddressPoints.AddressGrid",
        MatchAddress: "326 E SOUTH TEMPLE ST, SALT LAKE CITY",
        InputAddress: "326 east south temple, slc",
        AddressGrid: "SALT LAKE CITY",
        ScoreDifference: -1.0,
        Wkid: 26912,
        Candidates: [],
      },
    },
    status: 200,
  },
  singleEsriJsonWithCandidates: {
    result: {
      attributes: {
        Location: { x: 425608.67349758215, y: 4513499.5088544935 },
        Score: 98.24,
        Locator: "AddressPoints.AddressGrid",
        MatchAddress: "326 E SOUTH TEMPLE ST, SALT LAKE CITY",
        InputAddress: "326 east south temple, slc",
        AddressGrid: "SALT LAKE CITY",
        ScoreDifference: -1.0,
        Wkid: 26912,
        Candidates: [
          {
            address: "326 E ST, SALT LAKE CITY",
            location: { x: 425951.76055283466, y: 4514356.0690138135 },
            score: 94.6,
            locator: "Centerlines.StatewideRoads",
            addressGrid: "SALT LAKE CITY",
          },
        ],
      },
      geometry: {
        x: 425608.67349758215,
        y: 4513499.5088544935,
        type: "point",
        spatialReference: { wkid: 26912 },
      },
    },
    status: 200,
  },
};
