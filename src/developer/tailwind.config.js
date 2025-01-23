import ugrcPreset from '@ugrc/tailwind-preset';
import animate from 'tailwindcss-animate';
import rac from 'tailwindcss-react-aria-components';
import colors from 'tailwindcss/colors';

/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{tsx,jsx,js}', './node_modules/@ugrc/**/*.{tsx,jsx,js}'],
  darkMode: 'class',
  presets: [ugrcPreset],
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#f2f9f9',
          100: '#ddeef0',
          200: '#bedce3',
          300: '#91c3cf',
          400: '#5da1b3',
          500: '#428698',
          600: '#396e81',
          700: '#345b6a',
          // generated from
          800: '#33505d',
          900: '#2c424d',
          950: '#192933',
        },
        secondary: {
          50: '#fdfced',
          100: '#f7f4ce',
          200: '#efe898',
          300: '#e7d762',
          400: '#e3c949',
          500: '#d9ab27',
          600: '#c0871f',
          700: '#a0641d',
          800: '#824f1e',
          900: '#6b411c',
        },
        warning: colors.fuchsia,
      },
      fontFamily: {
        utah: ['"Source Sans 3"', '"Source Sans Pro"', 'Helvetica', 'Arial', 'sans-serif'],
      },
      animation: {
        'gradient-x': 'gradient-x 4s ease infinite',
      },
      keyframes: {
        'gradient-x': {
          '0%, 100%': {
            'background-size': '200% 200%',
            'background-position': 'left center',
          },
          '50%': {
            'background-size': '200% 200%',
            'background-position': 'right center',
          },
        },
      },
    },
  },
  plugins: [rac, animate],
};
