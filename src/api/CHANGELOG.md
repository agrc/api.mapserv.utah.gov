# Changelog

## [1.16.8](https://github.com/agrc/api.mapserv.utah.gov/compare/api-v1.16.7...api-v1.16.8) (2024-08-06)


### Bug Fixes

* **api:** don't direct users to explorer for testing ([bf785ad](https://github.com/agrc/api.mapserv.utah.gov/commit/bf785ad7136929f19e411b970695e50be98abb56))

## [1.16.7](https://github.com/agrc/api.mapserv.utah.gov/compare/api-v1.16.6...api-v1.16.7) (2024-08-06)


### Bug Fixes

* **api:** correct boolean logic to remove unnecessary log statement ([bb1708d](https://github.com/agrc/api.mapserv.utah.gov/commit/bb1708d99cf77206bdc44392145ea20982d4ee0b))

## [1.16.6](https://github.com/agrc/api.mapserv.utah.gov/compare/api-v1.16.5...api-v1.16.6) (2024-08-06)


### Bug Fixes

* **api:** add preserveCollapsed to preserve small geometries ([390e230](https://github.com/agrc/api.mapserv.utah.gov/commit/390e230e54ec37eff918ea761dafd1ce7dd9528c))

## [1.16.5](https://github.com/agrc/api.mapserv.utah.gov/compare/api-v1.16.4...api-v1.16.5) (2024-08-06)


### Bug Fixes

* **api:** fix raster queries starting with `sgid10.` or odd casings ([d16139d](https://github.com/agrc/api.mapserv.utah.gov/commit/d16139d0fa6596a24d6f9d1b8f330d1e5251cb71))

## [1.16.4](https://github.com/agrc/api.mapserv.utah.gov/compare/api-v1.16.3...api-v1.16.4) (2024-08-06)


### Bug Fixes

* **api:** allow raster queries to pass through table check ([2e7db67](https://github.com/agrc/api.mapserv.utah.gov/commit/2e7db6732da0f11a2f32bf1ba4bd1a58b3adca77))

## [1.16.3](https://github.com/agrc/api.mapserv.utah.gov/compare/api-v1.16.2...api-v1.16.3) (2024-08-06)


### Bug Fixes

* **api:** handle error where user provides empty point input ([ce94945](https://github.com/agrc/api.mapserv.utah.gov/commit/ce949458d167b39740a592656f7e6c0e8e8855f5))
* **api:** handle error where user provides wrong spatial reference ([ed98b8e](https://github.com/agrc/api.mapserv.utah.gov/commit/ed98b8ea57de0aaacf615c8449f5bbfe79b9f7d2))

## [1.16.2](https://github.com/agrc/api.mapserv.utah.gov/compare/api-v1.16.1...api-v1.16.2) (2024-08-05)


### Bug Fixes

* **api:** remove unused code ([58dd202](https://github.com/agrc/api.mapserv.utah.gov/commit/58dd202425a181166c61cafe9d7d8516ec6830c5))

## [1.16.1](https://github.com/agrc/api.mapserv.utah.gov/compare/api-v1.16.0...api-v1.16.1) (2024-07-02)


### Bug Fixes

* **api:** remove hardcoded dev project names ([35c7400](https://github.com/agrc/api.mapserv.utah.gov/commit/35c740077d252d3a9eba821708bc3be82c9d6715))

## [1.16.0](https://github.com/agrc/api.mapserv.utah.gov/compare/api-v1.15.0...api-v1.16.0) (2024-07-01)


### Features

* **api:** create feature class attribute endpoint ([f653f6d](https://github.com/agrc/api.mapserv.utah.gov/commit/f653f6d4e2f29fceb9b0c462139331ea74df6e1b)), closes [#298](https://github.com/agrc/api.mapserv.utah.gov/issues/298)
* **api:** create feature class names information endpoint ([099022c](https://github.com/agrc/api.mapserv.utah.gov/commit/099022c70ed4a0d91eda4cd726b69bad664b71d8)), closes [#297](https://github.com/agrc/api.mapserv.utah.gov/issues/297)


### Bug Fixes

* **api:** update links to new website ([557e8e9](https://github.com/agrc/api.mapserv.utah.gov/commit/557e8e91ab7e38db7dd6cef9a134e9b997642f31))
* **api:** update version in docker file ([76e710d](https://github.com/agrc/api.mapserv.utah.gov/commit/76e710d872007a53ef1685e37cb4d46cb847cf16))
* **explorer:** update links to new website ([c8475e8](https://github.com/agrc/api.mapserv.utah.gov/commit/c8475e88df384105f83bf97e85b3eff9402b9f24))
* **redis:** organize redis key names ([43713a9](https://github.com/agrc/api.mapserv.utah.gov/commit/43713a97f8a062129db24dcbe19d984407c572a1)), closes [#285](https://github.com/agrc/api.mapserv.utah.gov/issues/285)

## [1.15.0](https://github.com/agrc/api.mapserv.utah.gov/compare/api-v1.14.0...api-v1.15.0) (2023-10-31)


### Features

* **api:** add roads and highways as a health check ([7bcbfa2](https://github.com/agrc/api.mapserv.utah.gov/commit/7bcbfa22e8d7492260aab4e24674e2d6dac81443))
* **api:** create big query health check ([9edaf20](https://github.com/agrc/api.mapserv.utah.gov/commit/9edaf2032d1ab52aaf5fde43264752aa9884ee80))
* **api:** create raster elevation computation ([5a536a2](https://github.com/agrc/api.mapserv.utah.gov/commit/5a536a28eaa36ebad16febc7975ef8732be1bc7c))
* **esri:** add serializable graphic ([3c2e34a](https://github.com/agrc/api.mapserv.utah.gov/commit/3c2e34a76e1ca06c1c1c9c25c6ccb13d62fc3f4a))


### Bug Fixes

* **api:** add multipoint, line, and multiline to nts mapper ([c978c38](https://github.com/agrc/api.mapserv.utah.gov/commit/c978c38c20167a2107f182d0d3a8a438500b001b))
* **api:** correct concurrency serialization ([c42c3b0](https://github.com/agrc/api.mapserv.utah.gov/commit/c42c3b0e90afd720f0fd216a15fec281e9c6cd51))
* **api:** correct not found reverse geocode response ([e4fddf9](https://github.com/agrc/api.mapserv.utah.gov/commit/e4fddf9ff3be553115d41cbf8cace9ff56589031))
* **api:** correct output type for search geometry ([399efb3](https://github.com/agrc/api.mapserv.utah.gov/commit/399efb3652eef8665d6871a92512f0e58a0e8231))
* **api:** create an ip provider that works with our cloud infrastructure ([2a6cda1](https://github.com/agrc/api.mapserv.utah.gov/commit/2a6cda171e53642f009753b94471d9acd2a72a0f))
* **api:** fix nullability ([6aa895a](https://github.com/agrc/api.mapserv.utah.gov/commit/6aa895af5ae22e1c003abd39683e7649804aac71))
* **api:** provide proper options based on the major version ([e2a405d](https://github.com/agrc/api.mapserv.utah.gov/commit/e2a405dcfe63125df82d176cb2052b86d208581e))
* **api:** swap to new roads and highway service ([9a7b0e1](https://github.com/agrc/api.mapserv.utah.gov/commit/9a7b0e10403f8d2c909672cae9e14705fdcfb1b4))
* **api:** text queries are not case insensitive in postgres ([8cb6b75](https://github.com/agrc/api.mapserv.utah.gov/commit/8cb6b75635f792afba6a3f50a5e15b0fb00904a5))
* **api:** update bad request api key response message ([47a46d0](https://github.com/agrc/api.mapserv.utah.gov/commit/47a46d079861d1994b5dff982259183743e4efa2))
* **api:** update model binding to allow for api key authorization ([77a2b30](https://github.com/agrc/api.mapserv.utah.gov/commit/77a2b301f557833cb233c18ff676968d7bed56bc))

## [1.14.0](https://github.com/agrc/api.mapserv.utah.gov/compare/api-v1.13.6...api-v1.14.0) (2023-09-25)

### Features

- **api:** add roads and highways as a health check ([7bcbfa2](https://github.com/agrc/api.mapserv.utah.gov/commit/7bcbfa22e8d7492260aab4e24674e2d6dac81443))
- **api:** create big query health check ([9edaf20](https://github.com/agrc/api.mapserv.utah.gov/commit/9edaf2032d1ab52aaf5fde43264752aa9884ee80))
- **api:** create raster elevation computation ([5a536a2](https://github.com/agrc/api.mapserv.utah.gov/commit/5a536a28eaa36ebad16febc7975ef8732be1bc7c))
- **esri:** add serializable graphic ([3c2e34a](https://github.com/agrc/api.mapserv.utah.gov/commit/3c2e34a76e1ca06c1c1c9c25c6ccb13d62fc3f4a))

### Bug Fixes

- **api:** add multipoint, line, and multiline to nts mapper ([c978c38](https://github.com/agrc/api.mapserv.utah.gov/commit/c978c38c20167a2107f182d0d3a8a438500b001b))
- **api:** correct concurrency serialization ([c42c3b0](https://github.com/agrc/api.mapserv.utah.gov/commit/c42c3b0e90afd720f0fd216a15fec281e9c6cd51))
- **api:** correct not found reverse geocode response ([e4fddf9](https://github.com/agrc/api.mapserv.utah.gov/commit/e4fddf9ff3be553115d41cbf8cace9ff56589031))
- **api:** correct output type for search geometry ([399efb3](https://github.com/agrc/api.mapserv.utah.gov/commit/399efb3652eef8665d6871a92512f0e58a0e8231))
- **api:** create an ip provider that works with our cloud infrastructure ([2a6cda1](https://github.com/agrc/api.mapserv.utah.gov/commit/2a6cda171e53642f009753b94471d9acd2a72a0f))
- **api:** fix nullability ([6aa895a](https://github.com/agrc/api.mapserv.utah.gov/commit/6aa895af5ae22e1c003abd39683e7649804aac71))
- **api:** provide proper options based on the major version ([e2a405d](https://github.com/agrc/api.mapserv.utah.gov/commit/e2a405dcfe63125df82d176cb2052b86d208581e))
- **api:** text queries are not case insensitive in postgres ([8cb6b75](https://github.com/agrc/api.mapserv.utah.gov/commit/8cb6b75635f792afba6a3f50a5e15b0fb00904a5))
- **api:** update bad request api key response message ([47a46d0](https://github.com/agrc/api.mapserv.utah.gov/commit/47a46d079861d1994b5dff982259183743e4efa2))
- **api:** update model binding to allow for api key authorization ([77a2b30](https://github.com/agrc/api.mapserv.utah.gov/commit/77a2b301f557833cb233c18ff676968d7bed56bc))
- **api:** update reverse geocode json models for pro locators ([f4f8f30](https://github.com/agrc/api.mapserv.utah.gov/commit/f4f8f30e3ad8ad385b120d87c3f96a74ddc0d795))

## [1.1.1](https://github.com/agrc/api.mapserv.utah.gov/compare/api-v1.1.0...api-v1.1.1) (2023-09-25)

### Bug Fixes

- **api:** correct concurrency serialization ([c42c3b0](https://github.com/agrc/api.mapserv.utah.gov/commit/c42c3b0e90afd720f0fd216a15fec281e9c6cd51))
- **api:** create an ip provider that works with our cloud infrastructure ([2a6cda1](https://github.com/agrc/api.mapserv.utah.gov/commit/2a6cda171e53642f009753b94471d9acd2a72a0f))
- **api:** update bad request api key response message ([47a46d0](https://github.com/agrc/api.mapserv.utah.gov/commit/47a46d079861d1994b5dff982259183743e4efa2))
- **api:** update model binding to allow for api key authorization ([77a2b30](https://github.com/agrc/api.mapserv.utah.gov/commit/77a2b301f557833cb233c18ff676968d7bed56bc))

## [1.1.0](https://github.com/agrc/api.mapserv.utah.gov/compare/api-v1.0.0...api-v1.1.0) (2023-07-14)

### Features

- **api:** add roads and highways as a health check ([7bcbfa2](https://github.com/agrc/api.mapserv.utah.gov/commit/7bcbfa22e8d7492260aab4e24674e2d6dac81443))
- **api:** create big query health check ([9edaf20](https://github.com/agrc/api.mapserv.utah.gov/commit/9edaf2032d1ab52aaf5fde43264752aa9884ee80))
- **api:** create raster elevation computation ([5a536a2](https://github.com/agrc/api.mapserv.utah.gov/commit/5a536a28eaa36ebad16febc7975ef8732be1bc7c))
- **esri:** add serializable graphic ([3c2e34a](https://github.com/agrc/api.mapserv.utah.gov/commit/3c2e34a76e1ca06c1c1c9c25c6ccb13d62fc3f4a))

### Bug Fixes

- **api:** add multipoint, line, and multiline to nts mapper ([c978c38](https://github.com/agrc/api.mapserv.utah.gov/commit/c978c38c20167a2107f182d0d3a8a438500b001b))
- **api:** correct not found reverse geocode response ([e4fddf9](https://github.com/agrc/api.mapserv.utah.gov/commit/e4fddf9ff3be553115d41cbf8cace9ff56589031))
- **api:** correct output type for search geometry ([399efb3](https://github.com/agrc/api.mapserv.utah.gov/commit/399efb3652eef8665d6871a92512f0e58a0e8231))
- **api:** create rings properly ([98d9e23](https://github.com/agrc/api.mapserv.utah.gov/commit/98d9e2398964e6af6c8255179883e7ebcbfda8d6))
- **api:** do not register decorators as computation handler ([94cd482](https://github.com/agrc/api.mapserv.utah.gov/commit/94cd482ee96b6628afcef385e07f78ae36c3f489))
- **api:** fix nullability ([6aa895a](https://github.com/agrc/api.mapserv.utah.gov/commit/6aa895af5ae22e1c003abd39683e7649804aac71))
- **api:** ignore case when parsting attribute style ([18f30e1](https://github.com/agrc/api.mapserv.utah.gov/commit/18f30e1a2d41cc852788d91d1864a1f246df2451))
- **api:** provide proper options based on the major version ([e2a405d](https://github.com/agrc/api.mapserv.utah.gov/commit/e2a405dcfe63125df82d176cb2052b86d208581e))
- **api:** remove extra spaces in geometry input ([f983e44](https://github.com/agrc/api.mapserv.utah.gov/commit/f983e4418a6043dcbad12baa82cef0451ef33553))
- **api:** text queries are not case insensitive in postgres ([8cb6b75](https://github.com/agrc/api.mapserv.utah.gov/commit/8cb6b75635f792afba6a3f50a5e15b0fb00904a5))
- **api:** update reverse geocode json models for pro locators ([f4f8f30](https://github.com/agrc/api.mapserv.utah.gov/commit/f4f8f30e3ad8ad385b120d87c3f96a74ddc0d795))
- **api:** update tokens to match shape name ([e42d438](https://github.com/agrc/api.mapserv.utah.gov/commit/e42d438bf64b7381da9cbe64b4dec220398b200c))
- **ci:** auto accept apt update ([a3bdd5d](https://github.com/agrc/api.mapserv.utah.gov/commit/a3bdd5d52acde97a0ce290142bd730269eb35f31))
