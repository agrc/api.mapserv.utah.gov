---
title: "UGRC API"
description: "Help for wherever you are on your geospatial journey"
---

The UGRC API has 7 endpoints; 4 geocoding endpoints, 2 informational endpoints, and 1 searching endpoint.

### Geocoding

Geocoding allows you to get the geographic location (i.e., the geographical coordinates) from an address. The UGRC API endpoints for geocoding allow you to

- Get the coordinates of a street and zone
- Get the coordinates of a route and milepost
- Get the address of a coordinate
- Get the route and milepost of a coordinate
  {.list-disc}

### Searching

Searching allows you to search through more than [1,000,000 rows of spatial SGID data](https://gis.utah.gov/sgid). With over 300 layers of real-world data that you can run queries against in the SGID, you will be able to extract actionable information for your data. The UGRC API endpoint for searching allows you to

- Get the attributes of spatial data based on a T-SQL like query.
- Get the attributes of spatial data based on a location (i.e., spatial intersection).

To view the SGID tables that are accessible through the search endpoint, [connect to the OpenSGID](https://gis.utah.gov/sgid/open-sgid/). You can then browse everything that UGRC has to offer for the State of Utah.

### Meta

The informational endpoints allow you to inspect and learn about SGID data. If you do not wish to connect to the OpenSGID, you can learn about the same data through the API. The UGRC API endpoints for information allow you to

- Get a list of table names from our [data categories](https://gis.utah.gov/data/#data-categories).
- Get a list of attribute names from a table.
