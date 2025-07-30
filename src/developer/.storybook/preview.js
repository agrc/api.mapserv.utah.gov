/** @type { import('@storybook/react-vite').Preview } */
import { withThemeByClassName } from '@storybook/addon-themes';
import '../src/index.css';

export const parameters = {
  controls: {
    expanded: true,
    hideNoControlsWarning: true,
    matchers: {
      color: /(background|color)$/i,
      date: /Date$/,
    },
  },
};

export const decorators = [
  withThemeByClassName({
    themes: {
      light: '',
      dark: 'dark',
    },
    defaultTheme: 'light',
  }),
];
