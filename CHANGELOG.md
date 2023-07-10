# Changelog

## 1.0.0 (2023-07-10)


### ⚠ BREAKING CHANGES

* **api:** location removed from both geojson and esrijson output and properties camelcased.

### 📖 Documentation Improvements

* **api:** update docs with regard to k8s and terraform ([d5fc769](https://github.com/agrc/api.mapserv.utah.gov/commit/d5fc769441a6f8cc639165a25f725d17c3a8c0b2))
* **api:** update documentation ([1350ddf](https://github.com/agrc/api.mapserv.utah.gov/commit/1350ddfffd4d812c202df641932f3732ed7e0793))
* **forklift:** document pallet requirements ([0eed4ac](https://github.com/agrc/api.mapserv.utah.gov/commit/0eed4acc2525bce56551d160c475508535707dfe))
* update badges for github actions ([822d803](https://github.com/agrc/api.mapserv.utah.gov/commit/822d8037b619e632964ecd4da2814124952fd22d))
* update infrastructure and configuration docs ([6f5357d](https://github.com/agrc/api.mapserv.utah.gov/commit/6f5357db3b7901b738dd17d0e335e5c04d48767d))


### 🌲 Dependencies

* **api:** update packages ([ee38d21](https://github.com/agrc/api.mapserv.utah.gov/commit/ee38d2124fd060c48bbb6e3ddd7fb6fc098168f9))
* **developer:** update firebase tools ([7d7a63b](https://github.com/agrc/api.mapserv.utah.gov/commit/7d7a63b63b4702c3e04e4acd05e461acc45dab97))
* **developer:** update packages ([f51c73f](https://github.com/agrc/api.mapserv.utah.gov/commit/f51c73f0cfae5043fae3a09f7a16b3fa441ec117))
* **developer:** update packages ([7b6189a](https://github.com/agrc/api.mapserv.utah.gov/commit/7b6189a33b142002e5728715776d1ce21496f659))
* **developer:** update packages ([f94fcc6](https://github.com/agrc/api.mapserv.utah.gov/commit/f94fcc6031266c3cbe47b3966ae72b1a58140f7e))


### 🐛 Bug Fixes

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
* **ci:** remove quotes? ([5d51d84](https://github.com/agrc/api.mapserv.utah.gov/commit/5d51d84745fddeb5b709419cd3bdda6a310ad756))
* **explorer:** encode url parameters ([9813644](https://github.com/agrc/api.mapserv.utah.gov/commit/9813644e05505386e16e3a3bc765975247bb1446))
* **explorer:** remove unused class names ([81c8699](https://github.com/agrc/api.mapserv.utah.gov/commit/81c8699e126e73fe55e4f9e6bf53e2f35189f40a))
* **explorer:** update key to fix display ([98d153c](https://github.com/agrc/api.mapserv.utah.gov/commit/98d153cea936b02351a14200af51d61ce222decd))
* **k8s:** do not ignore folder containing pgsql data ([5541103](https://github.com/agrc/api.mapserv.utah.gov/commit/5541103b023b1d8118f324d6127ddf630e1fef5e))
* **k8s:** update health check ([2f3be4d](https://github.com/agrc/api.mapserv.utah.gov/commit/2f3be4dcaea089302c7971642881546f6fbe4e21))
* **k8s:** update ingress for routing ([68a3522](https://github.com/agrc/api.mapserv.utah.gov/commit/68a3522a2e278d26bfd3924935a78b40d2bef0af))
* **migration:** set environment ([09b7d1f](https://github.com/agrc/api.mapserv.utah.gov/commit/09b7d1fe87f37a60de16672b351c831493a9dacf))
* **terraform:** add nat router so private pods can reach external resources ([5d8c564](https://github.com/agrc/api.mapserv.utah.gov/commit/5d8c5644fc84e76fd58fb3a27d0cc2d007a8f094))


### 🚀 Features

* **api:** add roads and highways as a health check ([7bcbfa2](https://github.com/agrc/api.mapserv.utah.gov/commit/7bcbfa22e8d7492260aab4e24674e2d6dac81443))
* **api:** create big query health check ([9edaf20](https://github.com/agrc/api.mapserv.utah.gov/commit/9edaf2032d1ab52aaf5fde43264752aa9884ee80))
* **api:** create raster elevation computation ([5a536a2](https://github.com/agrc/api.mapserv.utah.gov/commit/5a536a28eaa36ebad16febc7975ef8732be1bc7c))
* **api:** udpate output formatters ([9848853](https://github.com/agrc/api.mapserv.utah.gov/commit/9848853cf9d6b1fdfc52c44a9663882bdd4de493))
* bring esrijson.net into project ([774fa89](https://github.com/agrc/api.mapserv.utah.gov/commit/774fa89102f3516b1f02deef9cdf6be36600ddbb))
* **esri:** add serializable graphic ([3c2e34a](https://github.com/agrc/api.mapserv.utah.gov/commit/3c2e34a76e1ca06c1c1c9c25c6ccb13d62fc3f4a))
* **explorer:** add ability to deep link to endpoint api/docs ([9b632ba](https://github.com/agrc/api.mapserv.utah.gov/commit/9b632baec292b60ab5f7bd48661ce82ea9f10ef1))
* **explorer:** add api version url parameter to documentation route ([#124](https://github.com/agrc/api.mapserv.utah.gov/issues/124)) ([a93f1b2](https://github.com/agrc/api.mapserv.utah.gov/commit/a93f1b27a970d1bbc867806014ce03ef0d173ab6)), closes [#121](https://github.com/agrc/api.mapserv.utah.gov/issues/121)
* **explorer:** add encoding warning for street value ([128a3aa](https://github.com/agrc/api.mapserv.utah.gov/commit/128a3aa78ec03f70c6af23bd51ba47b80c205184))
* **explorer:** allow for className to be passed to Tip component ([de13557](https://github.com/agrc/api.mapserv.utah.gov/commit/de135575b10e9fc0925bd13171f38f085ead25f8))
* **explorer:** display endpoint fetch errors rather than ignore them ([a88e3d9](https://github.com/agrc/api.mapserv.utah.gov/commit/a88e3d9e45e8ba1a3e9a217c500195b12544b19f))
* **explorer:** make docs labels deep-linkable ([75bce50](https://github.com/agrc/api.mapserv.utah.gov/commit/75bce5084c4eecd593354bfb7439c89f873654e5))
* **explorer:** refactor deep links into component and integrate with others ([f6d04eb](https://github.com/agrc/api.mapserv.utah.gov/commit/f6d04eba0a72a07e6f75e50316f8999685fb4d94))
* **k8s:** set some resource limits and requests ([e0c6735](https://github.com/agrc/api.mapserv.utah.gov/commit/e0c6735dc718662908d556223d077b4469635e37))


### 🎨 Design Improvements

* **api:** appease the analyzers ([84a893a](https://github.com/agrc/api.mapserv.utah.gov/commit/84a893adbec2d3804f8ed0273d92e69b63cb3ebd))
* **api:** casing and grammar ([c3c9555](https://github.com/agrc/api.mapserv.utah.gov/commit/c3c9555d38d06f8e4d4858c996908ff178470966))
* **api:** fix spelling ([f3c7484](https://github.com/agrc/api.mapserv.utah.gov/commit/f3c748492cc7057249b7b72c63bf1b27ff7eaee1))
* **api:** line continuations ([d63a51a](https://github.com/agrc/api.mapserv.utah.gov/commit/d63a51ac62cfa9eaddb859a7f6fcd0e7d4b2b42b))
* **api:** mark unused vars ([4b8fc77](https://github.com/agrc/api.mapserv.utah.gov/commit/4b8fc779a7aa34aad0d211a764b088e2d169a7ef))
* **api:** primary constructor and variable names ([f87ad46](https://github.com/agrc/api.mapserv.utah.gov/commit/f87ad461cad6680448e9a5f5c78ee52c594743c0))
* **api:** primary constructors ([19a6fe6](https://github.com/agrc/api.mapserv.utah.gov/commit/19a6fe6c0b25cfaa4b1dc3a958c83a2882986295))
* **api:** primary constructors ([de3bb6a](https://github.com/agrc/api.mapserv.utah.gov/commit/de3bb6a2f13d4c2b0485d8df2c6fc56b28600913))
* **api:** remove unused usings ([949e3c8](https://github.com/agrc/api.mapserv.utah.gov/commit/949e3c8a0c9a4bce062d5e9d0bc61ff7283a3779))
* **api:** remove unused usings ([6a63a33](https://github.com/agrc/api.mapserv.utah.gov/commit/6a63a33e566a3e6c6eb30e29045c48247090332b))
* **api:** top level constructor ([6cab8f7](https://github.com/agrc/api.mapserv.utah.gov/commit/6cab8f76dafb71369b1d99c7438760e4c11b1e5a))
* **api:** top level constructors, statics, and new()'s ([6c3011e](https://github.com/agrc/api.mapserv.utah.gov/commit/6c3011e495db8282540bdb3e50b49b99878c1694))
* **api:** use primary constructors ([3ff8207](https://github.com/agrc/api.mapserv.utah.gov/commit/3ff8207f6a857545c30be1aa4ceb9687a451fac1))
* **api:** use range operator ([1f1e7d8](https://github.com/agrc/api.mapserv.utah.gov/commit/1f1e7d888c7799a68cd7e112dcc9fa9423f77d45))
* **dashboard:** add design system fonts ([5f319fd](https://github.com/agrc/api.mapserv.utah.gov/commit/5f319fded5563c7d414caa94c478b36c5a16f082))
* **explorer:** make table of contents match the left sidebar ([74fa216](https://github.com/agrc/api.mapserv.utah.gov/commit/74fa216c7d6ed75242e2e91067d4053d6c18ecaf))
* **explorer:** update more menu and table of content styles ([71accc3](https://github.com/agrc/api.mapserv.utah.gov/commit/71accc3191438c337772b0cb38ee6bcde8613732))
* **explorer:** use demo instead of api ([72b970f](https://github.com/agrc/api.mapserv.utah.gov/commit/72b970f46b363e64a1e293d051367ed88cb9dd8a))
* **explorer:** use kebab case for endpoint id ([960fa26](https://github.com/agrc/api.mapserv.utah.gov/commit/960fa26832589706fe30b38b81867f4fa8152e7a))
* fix issues ([801b468](https://github.com/agrc/api.mapserv.utah.gov/commit/801b46883943baf168b056b3f6be5176a68a869b))
* **forklift:** type and format pallet ([3075d2c](https://github.com/agrc/api.mapserv.utah.gov/commit/3075d2ce670e8d4ef8eff0672c0f09d2025f13d4))
* **libs:** top level namespace ([8046268](https://github.com/agrc/api.mapserv.utah.gov/commit/8046268460194903d2877c2281a733bb80e1289d))
* remove unused usings ([d09fab2](https://github.com/agrc/api.mapserv.utah.gov/commit/d09fab242a6bce6cdcaaf822bb37e62664820f19))
* set charset to utf8 and use LF for ([341a0e0](https://github.com/agrc/api.mapserv.utah.gov/commit/341a0e0ffe0dedc33bce82738e648190d6a72f8a))
* update project settings ([7abbd04](https://github.com/agrc/api.mapserv.utah.gov/commit/7abbd046a87cd0b8e955644676d3262d7871aba4))
* update tip message ([af99cca](https://github.com/agrc/api.mapserv.utah.gov/commit/af99ccac0b262b9f63756795fafa86368e205e7b))
* use shorthand ([d4c8ca1](https://github.com/agrc/api.mapserv.utah.gov/commit/d4c8ca14a14a48afe5e43de649893230e83e25c6))
