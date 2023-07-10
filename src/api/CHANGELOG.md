# Changelog

## 1.0.0 (2023-07-10)


### ⚠ BREAKING CHANGES

* **api:** location removed from both geojson and esrijson output and properties camelcased.

### Features

* **api:** add roads and highways as a health check ([7bcbfa2](https://github.com/agrc/api.mapserv.utah.gov/commit/7bcbfa22e8d7492260aab4e24674e2d6dac81443))
* **api:** create big query health check ([9edaf20](https://github.com/agrc/api.mapserv.utah.gov/commit/9edaf2032d1ab52aaf5fde43264752aa9884ee80))
* **api:** create raster elevation computation ([5a536a2](https://github.com/agrc/api.mapserv.utah.gov/commit/5a536a28eaa36ebad16febc7975ef8732be1bc7c))
* **api:** udpate output formatters ([9848853](https://github.com/agrc/api.mapserv.utah.gov/commit/9848853cf9d6b1fdfc52c44a9663882bdd4de493))
* **esri:** add serializable graphic ([3c2e34a](https://github.com/agrc/api.mapserv.utah.gov/commit/3c2e34a76e1ca06c1c1c9c25c6ccb13d62fc3f4a))


### Bug Fixes

* **api:** 0 mileposts are not serialized as default so set to 0.001 ([0ad7e3f](https://github.com/agrc/api.mapserv.utah.gov/commit/0ad7e3f4ae11283afd84716479ef63ced3fb8c2b))
* **api:** add multipoint, line, and multiline to nts mapper ([c978c38](https://github.com/agrc/api.mapserv.utah.gov/commit/c978c38c20167a2107f182d0d3a8a438500b001b))
* **api:** branch on tryparse logic ([2100acb](https://github.com/agrc/api.mapserv.utah.gov/commit/2100acb34de8fffe08b0422de5099d2ab559d5b5))
* **api:** cancellation token forwarding only works in net5 ([a5a527f](https://github.com/agrc/api.mapserv.utah.gov/commit/a5a527f08fb06e59f900b7e4f224f6f75be12899))
* **api:** correct not found reverse geocode response ([e4fddf9](https://github.com/agrc/api.mapserv.utah.gov/commit/e4fddf9ff3be553115d41cbf8cace9ff56589031))
* **api:** correct output type for search geometry ([399efb3](https://github.com/agrc/api.mapserv.utah.gov/commit/399efb3652eef8665d6871a92512f0e58a0e8231))
* **api:** create rings properly ([98d9e23](https://github.com/agrc/api.mapserv.utah.gov/commit/98d9e2398964e6af6c8255179883e7ebcbfda8d6))
* **api:** do not register decorators as computation handler ([94cd482](https://github.com/agrc/api.mapserv.utah.gov/commit/94cd482ee96b6628afcef385e07f78ae36c3f489))
* **api:** fix ability to calculate score difference when suggest is 0 ([374c244](https://github.com/agrc/api.mapserv.utah.gov/commit/374c244ec1c90560d509a64f8d4dfe6b810cc84b))
* **api:** fix missing suggestions ([5d57fae](https://github.com/agrc/api.mapserv.utah.gov/commit/5d57faef1198af68ecab68ef1802fc0b84128463))
* **api:** fix nullability ([6aa895a](https://github.com/agrc/api.mapserv.utah.gov/commit/6aa895af5ae22e1c003abd39683e7649804aac71))
* **api:** handle error responses ([0709b4b](https://github.com/agrc/api.mapserv.utah.gov/commit/0709b4badf73364ec07c5b59ea104ea697a05940))
* **api:** ignore case when parsting attribute style ([18f30e1](https://github.com/agrc/api.mapserv.utah.gov/commit/18f30e1a2d41cc852788d91d1864a1f246df2451))
* **api:** log invalid request ([63fa9af](https://github.com/agrc/api.mapserv.utah.gov/commit/63fa9af29e58ed4342b241dadf08f19861617ddf))
* **api:** only add values without jsonignore attribute ([1cd45a8](https://github.com/agrc/api.mapserv.utah.gov/commit/1cd45a815a9a4ace7bd6348e24a7f4d6c42c1ef7))
* **api:** provide proper options based on the major version ([e2a405d](https://github.com/agrc/api.mapserv.utah.gov/commit/e2a405dcfe63125df82d176cb2052b86d208581e))
* **api:** refactor filtering to better include all udot routes ([3daa07f](https://github.com/agrc/api.mapserv.utah.gov/commit/3daa07fe4d777e5e88bc7f22a8bffb42d437d503))
* **api:** remove extra spaces in geometry input ([f983e44](https://github.com/agrc/api.mapserv.utah.gov/commit/f983e4418a6043dcbad12baa82cef0451ef33553))
* **api:** reverse milepost issues with non concurrencies ([8dceb73](https://github.com/agrc/api.mapserv.utah.gov/commit/8dceb731fb9d5f38b3241957770d25fd486ee9b9))
* **api:** text queries are not case insensitive in postgres ([8cb6b75](https://github.com/agrc/api.mapserv.utah.gov/commit/8cb6b75635f792afba6a3f50a5e15b0fb00904a5))
* **api:** to and from should be equal ([cc27767](https://github.com/agrc/api.mapserv.utah.gov/commit/cc27767dd2f29ef08ecb9f7a2a7b33f3c02c3ea1))
* **api:** update reverse geocode json models for pro locators ([f4f8f30](https://github.com/agrc/api.mapserv.utah.gov/commit/f4f8f30e3ad8ad385b120d87c3f96a74ddc0d795))
* **api:** update tokens to match shape name ([e42d438](https://github.com/agrc/api.mapserv.utah.gov/commit/e42d438bf64b7381da9cbe64b4dec220398b200c))
* **api:** use hashcode so duplicate distances are included in set ([7317071](https://github.com/agrc/api.mapserv.utah.gov/commit/73170717a042c92c6b93d0ad2b1328f04731b45e))
* **api:** use route instead of hashcode ([821e0e1](https://github.com/agrc/api.mapserv.utah.gov/commit/821e0e10c10b1a463faf815a8d36598b515381df))
* **ci:** auto accept apt update ([a3bdd5d](https://github.com/agrc/api.mapserv.utah.gov/commit/a3bdd5d52acde97a0ce290142bd730269eb35f31))
