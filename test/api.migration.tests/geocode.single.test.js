import { describe, expect, it } from "vitest";
import got from "got";
import fs from "fs";
import responses from "./responses";

const searchParams = {
  apiKey: "agrc-dev",
};
const headers = {
  referer: "http://unit.tests",
};
const throwHttpErrors = false;
const retry = {
  limit: 0,
};
const urls = {
  match: "326 east south temple/slc",
  noMatch: "326 east south temple st/provo",
};

const getResponseFor = (name) =>
  fs.readFileSync(`responses/${name}`, "utf-8").trim();

const prefixUrl = "https://ut-dts-agrc-web-api-dev.web.app/api/v1/geocode/";

const cloudClient = got.extend({
  prefixUrl: "http://localhost:1337/api/v1/geocode/",
  searchParams,
  headers,
  throwHttpErrors,
  retry,
});

describe("geocoding", () => {
  it.concurrent("single", async () => {
    const cloud = await cloudClient.get(urls.match).json();

    expect(cloud).toEqual(responses.single);
  });
  it.concurrent("single.404", async () => {
    const cloud = await cloudClient.get(urls.noMatch).json();

    expect(cloud).toEqual(responses.single404);
  });

  describe("with callback", () => {
    it.concurrent("single", async () => {
      const searchParams = {
        callback: "jsonp",
      };

      const agrc = getResponseFor("single.w.callback.txt");
      const cloud = await cloudClient
        .get(urls.match, {
          searchParams,
        })
        .text();

      expect(cloud).toEqual(agrc);
    });
  });

  describe("format", () => {
    it.concurrent("esrijson", async () => {
      const searchParams = {
        format: "esrijson",
      };

      const cloud = await cloudClient
        .get(urls.match, {
          searchParams,
        })
        .json();

      expect(cloud).toEqual(responses.singleEsriJson);
    });
    it.concurrent("esrijson-404", async () => {
      const searchParams = {
        format: "esrijson",
      };

      const cloud = await cloudClient
        .get(urls.noMatch, {
          searchParams,
        })
        .json();

      expect(cloud).toEqual(responses.single404);
    });
    it.concurrent("geojson", async () => {
      const searchParams = {
        format: "geojson",
      };

      const cloud = await cloudClient
        .get(urls.match, {
          searchParams,
        })
        .json();

      expect(cloud).toEqual(responses.singleGeoJson);
    });

    describe("with candidates", () => {
      it.concurrent("esrijson", async () => {
        const searchParams = {
          format: "esrijson",
          suggest: 1,
        };

        const cloud = await cloudClient
          .get(urls.match, {
            searchParams,
          })
          .json();

        expect(cloud).toEqual(responses.singleEsriJsonWithCandidates);
      });
    });
  });
});
