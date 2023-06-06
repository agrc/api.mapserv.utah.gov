import { describe, expect, it } from "vitest";
import got from "got";
import fs from "fs";
import search from "./responses/search";

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
const url = "boundaries.county_boundaries/";

const getResponseFor = (name) =>
  fs.readFileSync(`responses/geocode/single/${name}`, "utf-8").trim();

const prefixUrl = "https://ut-dts-agrc-web-api-dev.web.app/api/v1/geocode/";

const cloudClient = got.extend({
  prefixUrl: "http://localhost:1337/api/v1/search/",
  searchParams,
  headers,
  throwHttpErrors,
  retry,
});

describe("search", () => {
  describe("attribute", () => {
    it.concurrent("=", async () => {
      const cloud = await cloudClient
        .get(`${url}name`, {
          searchParams: {
            predicate: `name='KANE'`,
          },
        })
        .json();

      expect(cloud).toEqual(search.attributeWithPredicate);
    });
    it.concurrent("LIKE", async () => {
      const cloud = await cloudClient
        .get(`${url}name`, {
          searchParams: {
            predicate: `name LIKE 'K%'`,
          },
        })
        .json();

      expect(cloud).toEqual(search.attributeWithPredicate);
    });
    it.concurrent("no results", async () => {
      const cloud = await cloudClient
        .get(`${url}name`, {
          searchParams: {
            predicate: `name='NOPE'`,
          },
        })
        .json();

      expect(cloud).toEqual(search.empty);
    });
  });
  describe("geometry", () => {
    it.concurrent("atlas county", async () => {
      const cloud = await cloudClient
        .get(`${url}name`, {
          searchParams: {
            geometry:
              'point: {"spatialReference":{"latestWkid":3857,"wkid":102100},"x":-12462628.917113958,"y":5044995.095742975}',
            spatialReference: 3857,
          },
        })
        .json();

      expect(cloud).toEqual(search.atlas.county);
    });

    it.concurrent("atlas national grid", async () => {
      const cloud = await cloudClient
        .get(`indices.national_grid/grid1mil,grid100k`, {
          searchParams: {
            geometry:
              'point: {"spatialReference":{"latestWkid":3857,"wkid":102100},"x":-12462628.917113958,"y":5044995.095742975}',
            spatialReference: 3857,
          },
        })
        .json();

      expect(cloud).toEqual(search.atlas.nationalGrid);
    });
  });
});
