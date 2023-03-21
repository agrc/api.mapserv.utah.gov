import { assert, describe, expect, it } from "vitest";
import got from "got";

const searchParams = {
  apiKey: "agrc-dev",
};
const headers = {
  referer: "http://unit.tests",
};
const throwHttpErrors = false;

const client = got.extend({
  prefixUrl: "https://api.mapserv.utah.gov/api/v1/geocode/",
  searchParams,
  headers,
  throwHttpErrors,
});

// const cloudClient = got.extend({
//   prefixUrl: "https://ut-dts-agrc-web-api-dev.web.app/api/v1/geocode/",
//   searchParams,
// });

const cloudClient = got.extend({
  prefixUrl: "http://localhost:1337/api/v1/geocode/",
  searchParams,
  headers,
  throwHttpErrors,
});

describe("geocoding", () => {
  it.concurrent("single", async () => {
    const url = "326 east south temple/slc";

    const agrc = await client.get(url).json();
    const cloud = await cloudClient.get(url).json();

    expect(cloud).toEqual(agrc);
  });
  it.concurrent("single-404", async () => {
    const url = "326 east south temple/provo";

    const agrc = await client.get(url).json();
    const cloud = await cloudClient.get(url).json();

    expect(cloud).toEqual(agrc);
  });

  describe("format", () => {
    it.concurrent("esrijson", async () => {
      const url = "326 east south temple/slc";
      const searchParams = {
        format: "esrijson",
      };

      const agrc = await client
        .get(url, {
          searchParams,
        })
        .json();
      const cloud = await cloudClient
        .get(url, {
          searchParams,
        })
        .json();

      expect(cloud).toEqual(agrc);
    });
    it.concurrent("esrijson-404", async () => {
      const url = "326 east south temple/provo";
      const searchParams = {
        format: "esrijson",
      };

      const agrc = await client
        .get(url, {
          searchParams,
        })
        .json();
      const cloud = await cloudClient
        .get(url, {
          searchParams,
        })
        .json();

      expect(cloud).toEqual(agrc);
    });

    describe("with candidates", () => {
      it.concurrent("esrijson", async () => {
        const url = "326 east south temple/slc";
        const searchParams = {
          format: "esrijson",
          suggest: 1,
        };

        const agrc = await client
          .get(url, {
            searchParams,
          })
          .json();
        const cloud = await cloudClient
          .get(url, {
            searchParams,
          })
          .json();

        expect(cloud).toEqual(agrc);
      });
    });
  });
});
