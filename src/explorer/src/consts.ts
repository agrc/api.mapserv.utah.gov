export const SITE = {
  title: "UGRC API Explorer",
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

export const GITHUB_EDIT_URL = `https://github.com/agrc/api.mapserv.utahgov/tree/main/src/explorer`;

// See "Algolia" section of the README for more information.
export const ALGOLIA = {
  indexName: "XXXXXXXXXX",
  appId: "XXXXXXXXXX",
  apiKey: "XXXXXXXXXX",
};

export const SIDEBAR = {
  en: {
    Information: [
      { text: "Documentation", link: "en/introduction" },
      { text: "Getting Started Guide", link: "en/getting-started-guide" },
      { text: "Sample Usage", link: "en/sample-usage" },
      { text: "Release Notes", link: "en/release-notes" },
      { text: "Report a Problem", link: "en/page-3" },
      { text: "Privacy Policy", link: "en/privacy-policy" },
    ],
    Endpoints: [{ text: "Geocoding", link: "en/geocoding" }],
  },
};
