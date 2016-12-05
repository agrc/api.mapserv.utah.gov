# AGRC Web API

### web api installation

1. Clone https://github.com/steveoh/EsriJson and https://github.com/steveoh/GeoJSON.Net to `C:\GitHub`.
1. Create a webapi website in IIS and point to WebAPI.API and filter all requests to it.
1. Create a dashboard website in IIS and point it to WebAPI.Dashboard and filter all requests to it.
1. Install [RavenDB](https://ravendb.net/).
1. Create a new database called `WSUT` and import a `.ravendump` file from the server.
1. Install Redis from https://github.com/MSOpenTech/redis/releases accepting all default options.
1. Create a copy of `Soe.Common/App.sample.config` as `App.config` and fill in the
1. ArcGIS Server services
1. Add entries to hosts file
1. Build project in Visual Studio

![Powered by RavenDB](https://ravendb.net/Content/images/badges/badge1.png)
