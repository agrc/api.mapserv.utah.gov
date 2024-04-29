/** @type { import('@storybook/react').Preview } */
import { withThemeByClassName } from '@storybook/addon-styling';
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
