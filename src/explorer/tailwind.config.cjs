/** @type {import('tailwindcss').Config} */
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
    },
  },
  plugins: [],
};
