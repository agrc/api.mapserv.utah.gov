export const SITE = {
  title: "UGRC API Docs",
  description: "The description",
  defaultLanguage: "en-us",
} as const;

export const OPEN_GRAPH = {
  image: {
    src: "https://github.com/withastro/astro/blob/main/assets/social/banner-minimal.png?raw=true",
    alt:
      "astro logo on a starry expanse of space," +
      " with a purple saturn-like planet floating in the right foreground",
  },
  twitter: "MapUtah",
};

export const KNOWN_LANGUAGES = {
  English: "en",
} as const;
export const KNOWN_LANGUAGE_CODES = Object.values(KNOWN_LANGUAGES);
const branch = "development";
export const GITHUB_EDIT_URL = `https://github.com/agrc/api.mapserv.utah.gov/tree/${branch}/src/explorer`;

// See "Algolia" section of the README for more information.
export const ALGOLIA = {
  indexName: "XXXXXXXXXX",
  appId: "XXXXXXXXXX",
  apiKey: "XXXXXXXXXX",
};

export const SIDEBAR = {
  en: {
    Information: [
      { text: "Getting Started Guide", link: "getting-started" },
      {
        text: "Sample Code",
        link: "https://github.com/agrc/api.mapserv.utah.gov/tree/development/samples",
      },
      {
        text: "Release Notes",
        link: "https://github.com/agrc/api.mapserv.utah.gov/tags",
      },
      {
        text: "Report a Problem",
        link: "https://github.com/agrc/api.mapserv.utah.gov/issues/new",
      },
      { text: "Privacy Policy", link: "privacy-policy" },
    ],
    Endpoints: [{ text: "Geocoding", link: "en/geocoding" }],
  },
};
