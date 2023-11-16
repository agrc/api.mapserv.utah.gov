import starlightPlugin from "@astrojs/starlight-tailwind";
import heroPatterns from "tailwind-heropatterns";
import colors from "tailwindcss/colors";

// Generated color palettes
const mustard = {
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
};
const wavy = {
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
};

/** @type {import('tailwindcss').Config} */
export default {
  content: ["./src/**/*.{astro,html,js,jsx,md,mdx,svelte,ts,tsx,vue}"],
  theme: {
    extend: {
      fontFamily: {
        sans: [
          "ui-sans-serif",
          "system-ui",
          "-apple-system",
          "BlinkMacSystemFont",
          '"Segoe UI"',
          "Roboto",
          '"Helvetica Neue"',
          "Arial",
          '"Noto Sans"',
          "sans-serif",
          '"Apple Color Emoji"',
          '"Segoe UI Emoji"',
          '"Segoe UI Symbol"',
          '"Noto Color Emoji"',
        ],
        mono: [
          "IBM Plex Mono",
          "Consolas",
          "Andale Mono WT",
          "Andale Mono",
          "Lucida Console",
          "Lucida Sans Typewriter",
          "DejaVu Sans Mono",
          "Bitstream Vera Sans Mono",
          "Liberation Mono",
          "Nimbus Mono L",
          "Monaco",
          "Courier New",
          "Courier",
          "monospace",
        ],
      },
      colors: {
        accent: wavy,
        wavy,
        mustard,
      },
    },
  },
  plugins: [starlightPlugin()],
};
