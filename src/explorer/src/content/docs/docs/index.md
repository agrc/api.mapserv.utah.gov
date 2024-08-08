---
title: 'UGRC API'
description: 'Help for wherever you are on your geospatial journey'
next:
  { label: 'Street and zone geocoding', link: '/docs/v1/endpoints/geocoding' }
---

The UGRC API has 7 endpoints: 4 geocoding endpoints, 2 informational endpoints, and 1 searching endpoint.

### Geocoding

Geocoding allows you to get the geographic location (i.e., the geographical coordinates) from an address. You can learn so much from a home, business, or other type of address once it has been geocoded. When you geocode data, you unlock the ability to visualize the data in a different way. And you can use our search endpoint to answer endless questions about your addresses. The UGRC API endpoints for geocoding allow you to:

- Get the coordinates of a street and zone.
- Get the coordinates of a route and milepost.
- Get the address of a coordinate.
- Get the route and milepost of a coordinate.

### Searching

Searching allows you to search through more than 1,000,000 rows of [State Geographic Information Datasource](https://gis.utah.gov/documentation/sgid/) (SGID) data. With over **300** layers of real-world data that you can run queries against in the SGID, you will be able to extract actionable information for your data. The UGRC API endpoint for searching allows you to:

- Get the attributes of spatial data based on a T-SQL like query.
- Get the attributes of spatial data based on a location (i.e., spatial intersection).

To view the SGID tables that are accessible through the search endpoint, [connect to the Open SGID](https://gis.utah.gov/documentation/sgid/open-sgid/). You can then browse everything that UGRC has to offer for the State of Utah.

### Meta

The informational endpoints allow you to inspect and learn about SGID data. If you do not wish to connect to the Open SGID, you can learn about the same data through the API. The UGRC API endpoints for information allow you to:

- Get a list of table names from our [data categories](https://gis.utah.gov/products/sgid/categories/).
- Get a list of attribute names from a table.
