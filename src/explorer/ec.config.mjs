import { pluginEndpointHighlight } from './src/plugins/pluginEndpointHighlight.js';

/** @type {import('@astrojs/starlight/expressive-code').StarlightExpressiveCodeOptions} */
export default {
  plugins: [pluginEndpointHighlight()],
};
