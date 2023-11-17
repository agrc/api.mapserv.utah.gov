import starlight from "@astrojs/starlight";
import tailwind from "@astrojs/tailwind";
import { defineConfig } from "astro/config";
import starlightLinksValidator from "starlight-links-validator";

// https://astro.build/config
export default defineConfig({
  integrations: [
    starlightLinksValidator(),
    starlight({
      title: "UGRC API Documentation",
      site: "https://ut-dts-agrc-web-api-dev.web.app",
      logo: {
        src: "./src/assets/api.svg",
      },
      social: {
        github: "https://github.com/agrc/api.mapserv.utah.gov",
        "x.com": "https://x.com/maputah",
        facebook: "https://www.facebook.com/utahagrc",
      },
      sidebar: [
        {
          label: "Reference",
          items: [
            { label: "Getting Started Guide", link: "/getting-started/" },
            { label: "Documentation Overview", link: "/docs/" },
            {
              label: "Sample Code",
              link: "https://github.com/agrc/api.mapserv.utah.gov/tree/development/samples",
            },
            {
              label: "Release Notes",
              link: "https://github.com/agrc/api.mapserv.utah.gov/tags",
            },
            {
              label: "Report a Problem",
              link: "https://github.com/agrc/api.mapserv.utah.gov/issues/new",
            },
            { label: "Privacy Policy", link: "/privacy/" },
          ],
        },
        {
          label: "Endpoints (version 1)",
          autogenerate: { directory: "/docs/v1/endpoints/" },
        },
      ],
      customCss: ["./src/tailwind.css"],
    }),
    tailwind({ applyBaseStyles: true }),
  ],
});
