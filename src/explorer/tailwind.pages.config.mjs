/** @type {import('tailwindcss').Config} */
export default {
  content: [
    './src/**/*.{astro,html,js,jsx,md,mdx,ts,tsx}',
    './node_modules/@ugrc/**/*.{js,jsx,ts,tsx}',
  ],
  theme: {
    extend: {
      fontFamily: {
        sans: [
          'ui-sans-serif',
          'system-ui',
          '-apple-system',
          'BlinkMacSystemFont',
          '"Segoe UI"',
          'Roboto',
          '"Helvetica Neue"',
          'Arial',
          '"Noto Sans"',
          'sans-serif',
          '"Apple Color Emoji"',
          '"Segoe UI Emoji"',
          '"Segoe UI Symbol"',
          '"Noto Color Emoji"',
        ],
        mono: [
          'IBM Plex Mono',
          'Consolas',
          'Andale Mono WT',
          'Andale Mono',
          'Lucida Console',
          'Lucida Sans Typewriter',
          'DejaVu Sans Mono',
          'Bitstream Vera Sans Mono',
          'Liberation Mono',
          'Nimbus Mono L',
          'Monaco',
          'Courier New',
          'Courier',
          'monospace',
        ],
      },
    },
  },
};
