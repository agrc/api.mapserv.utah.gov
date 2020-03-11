# AGRC Web API Explorer

This project was bootstrapped with [Create React App](https://github.com/facebook/create-react-app) and is the main documentation and API exploration tool for [https://api.mapserv.utah.gov](https://api.mapserv.utah.gov).

## Deployment

This app runs as a docker container. To build the image `docker-compose build explorer`. To run the container `docker run -p 8080:80 apimapservutahgov_explorer`

## Development

### Available Scripts

In the project directory, you can run:

#### `npm start`

- Runs the app in the development mode.
- Open [http://localhost:3000](http://localhost:3000) to view it in the browser.

- The page will reload if you make edits.
- You will also see any lint errors in the console.

#### `npm test`

- Launches the test runner in the interactive watch mode.
- See the section about [running tests](https://facebook.github.io/create-react-app/docs/running-tests) for more information.

#### `npm run storybook`

This shows individual components and groups of components in isolation.

- Launches the storybook runner in the interactive watch mode
- Opens [http://localhost:9009](http://localhost:9009) to view it in the browser

- The page will reload if you make edits.

#### `npm run build`

- Builds the app for production to the `build` folder.
- It correctly bundles React in production mode and optimizes the build for the best performance.

- The build is minified and the filenames include the hashes.
- Your app is ready to be deployed!

See the section about [deployment](https://facebook.github.io/create-react-app/docs/deployment) for more information.

### Learn More

You can learn more in the [Create React App documentation](https://facebook.github.io/create-react-app/docs/getting-started).

To learn React, check out the [React documentation](https://reactjs.org/).

#### Code Splitting

This section has moved here: https://facebook.github.io/create-react-app/docs/code-splitting

#### Analyzing the Bundle Size

This section has moved here: https://facebook.github.io/create-react-app/docs/analyzing-the-bundle-size

#### Making a Progressive Web App

This section has moved here: https://facebook.github.io/create-react-app/docs/making-a-progressive-web-app

#### Advanced Configuration

This section has moved here: https://facebook.github.io/create-react-app/docs/advanced-configuration

#### `npm run build` fails to minify

This section has moved here: https://facebook.github.io/create-react-app/docs/troubleshooting#npm-run-build-fails-to-minify
