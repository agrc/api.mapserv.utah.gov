/** @type {import('tailwindcss').Config} */
const round = (num) =>
  num
    .toFixed(7)
    .replace(/(\.[0-9]+?)0+$/, "$1")
    .replace(/\.0$/, "");
const em = (px, base) => `${round(px / base)}em`;

module.exports = {
  content: ["./src/**/*.{astro,html,js,jsx,md,mdx,svelte,ts,tsx,vue}"],
  theme: {
    extend: {
      colors: {
        wavy: {
          50: "#f3f7f8",
          100: "#dfebee",
          200: "#c3d8de",
          300: "#9abcc6",
          400: "#6998a7",
          500: "#4d7b8d",
          600: "#456a7b",
          700: "#3b5563",
          800: "#364954",
          900: "#313f48",
        },
        mustard: {
          50: "#fdfced",
          100: "#f7f4ce",
          200: "#efe898",
          300: "#e7d762",
          400: "#e3c949",
          500: "#d9ab27",
          600: "#c0871f",
          700: "#a0641d",
          800: "#824f1e",
          900: "#6b411c",
        },
      },
      typography: ({ theme }) => ({
        DEFAULT: {
          css: {
            "blockquote p:first-of-type::before": { content: "none" },
            "code::before": { content: "none" },
            "code::after": { content: "none" },
            code: {
              color: "var(--tw-prose-code)",
              backgroundColor: "var(--tw-prose-pre-bg)",
              fontWeight: "600",
              padding: "0.1rem 0.25rem",
              borderRadius: "0.25rem",
            },
            "blockquote p:first-of-type::after": { content: "none" },
            "--tw-prose-body": theme("colors.slate[800]"),
            "--tw-prose-headings": theme("colors.slate[900]"),
            "--tw-prose-lead": theme("colors.slate[700]"),
            "--tw-prose-links": theme("colors.mustard[900]"),
            "--tw-prose-bold": theme("colors.slate[900]"),
            "--tw-prose-counters": theme("colors.slate[600]"),
            "--tw-prose-bullets": theme("colors.slate[400]"),
            "--tw-prose-hr": theme("colors.slate[300]"),
            "--tw-prose-quotes": theme("colors.slate[900]"),
            "--tw-prose-quote-borders": theme("colors.slate[300]"),
            "--tw-prose-captions": theme("colors.slate[700]"),
            "--tw-prose-code": theme("colors.slate[100]"),
            "--tw-prose-pre-code": theme("colors.mustard[900]"),
            "--tw-prose-pre-bg": theme("colors.slate[900]"),
            "--tw-prose-th-borders": theme("colors.slate[300]"),
            "--tw-prose-td-borders": theme("colors.slate[200]"),
            "--tw-prose-invert-body": theme("colors.slate[300]"),
            "--tw-prose-invert-headings": theme("colors.slate[300]"),
            "--tw-prose-invert-lead": theme("colors.slate[300]"),
            "--tw-prose-invert-links": theme("colors.mustard[500]"),
            "--tw-prose-invert-bold": theme("colors.white"),
            "--tw-prose-invert-counters": theme("colors.slate[400]"),
            "--tw-prose-invert-bullets": theme("colors.slate[600]"),
            "--tw-prose-invert-hr": theme("colors.slate[700]"),
            "--tw-prose-invert-quotes": theme("colors.slate[100]"),
            "--tw-prose-invert-quote-borders": theme("colors.slate[700]"),
            "--tw-prose-invert-captions": theme("colors.slate[400]"),
            "--tw-prose-invert-code": theme("colors.slate[800]"),
            "--tw-prose-invert-pre-code": theme("colors.slate[300]"),
            "--tw-prose-invert-pre-bg": theme("colors.slate[100]"),
            "--tw-prose-invert-th-borders": theme("colors.slate[600]"),
            "--tw-prose-invert-td-borders": theme("colors.slate[700]"),
          },
        },
      }),
    },
  },
  plugins: [require("@tailwindcss/typography")],
  darkMode: "class",
};
