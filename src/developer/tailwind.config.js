import ugrcPreset from '@ugrc/tailwind-preset';
import animate from 'tailwindcss-animate';
import rac from 'tailwindcss-react-aria-components';
import colors from 'tailwindcss/colors';

/** @type {import('tailwindcss').Config} */
export default {
  content: [
    './index.html',
    './src/**/*.{tsx,jsx,js}',
    './node_modules/@ugrc/**/*.{tsx,jsx,js}',
  ],
  darkMode: 'class',
  presets: [ugrcPreset],
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#f3f7f8',
          100: '#dfebee',
          200: '#c3d8de',
          300: '#9abcc6',
          400: '#6998a7',
          500: '#4d7b8d',
          600: '#456a7b',
          700: '#3b5563',
          800: '#364954',
          900: '#313f48',
        },
        mustard: {
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
        utah: [
          '"Source Sans 3"',
          '"Source Sans Pro"',
          'Helvetica',
          'Arial',
          'sans-serif',
        ],
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
