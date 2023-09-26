# Changelog

## [1.15.0](https://github.com/agrc/api.mapserv.utah.gov/compare/api-v1.14.0...api-v1.15.0) (2023-09-26)


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
* **api:** text queries are not case insensitive in postgres ([8cb6b75](https://github.com/agrc/api.mapserv.utah.gov/commit/8cb6b75635f792afba6a3f50a5e15b0fb00904a5))
* **api:** update bad request api key response message ([47a46d0](https://github.com/agrc/api.mapserv.utah.gov/commit/47a46d079861d1994b5dff982259183743e4efa2))
* **api:** update model binding to allow for api key authorization ([77a2b30](https://github.com/agrc/api.mapserv.utah.gov/commit/77a2b301f557833cb233c18ff676968d7bed56bc))
* **api:** update reverse geocode json models for pro locators ([f4f8f30](https://github.com/agrc/api.mapserv.utah.gov/commit/f4f8f30e3ad8ad385b120d87c3f96a74ddc0d795))

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
