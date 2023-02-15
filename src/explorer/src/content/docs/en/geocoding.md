---
title: "Geocoding Endpoints"
description: "Everything roads, highways, and addresses"
---

## Street and Zone

### Tips

1. This endpoint confirms that an address **can** exist along a road, however, there may be no structure or mail delivery at the address. This endpoint should not be used to validate mailing addresses.
1. [Reserved characters](https://www.rfc-editor.org/rfc/rfc3986#section-2.1), like ?, #, @, etc. in your data need to be escaped or the request will fail.
1. `matchAddress` returns the name of the address grid system for the address. For example, `"matchAddress": "10420 E Little Cottonwood Canyon, Salt Lake City"` means that the address is part of the **Salt Lake City** address grid system. It is neither within the boundaries of Salt Lake City proper, nor is that the preferred mailing address placename.

### URI Format

**HTTP Verb**: `GET`

```md
https://api.mapserv.utah.gov/api/v1/geocode/:street/:zone
```

### Required Fields

#### street

A Utah street address. eg: 326 east south temple st. A valid mailing address or structure does not need to exist at the input street to find a match. If the house number exists in the range of the street, a coordinate will be extrapolated from the road centerlines.

#### zone

A Utah municipality name or 5 digit zip code eg Provo or 84111.

### Optional Fields

#### spatialReference

The spatial reference defines how the coordinates will represent a location on the earth defined by how the round earth was made flat. The well known id's (WKID) of different coordinate systems define if the coordinates will be stored as degrees of longitude and latitude, meters, feet, etc. This endpoint supports the WKIDs from the [Geographic Coordinate System](https://desktop.arcgis.com/en/arcmap/10.5/map/projections/pdf/geographic_coordinate_systems.pdf) reference and the [Projected Coordinate System](https://desktop.arcgis.com/en/arcmap/10.5/map/projections/pdf/projected_coordinate_systems.pdf) reference. UTM Zone 12 N, with the WKID of **26912**, is the default. This coordinate system is the most accurate reflection of Utah. It is recommended to use this coordinate system if length and area calculations are important as other coordinate systems will skew the truth.

##### Popular Spatial Reference Ids

| WKID  | NAME                       | REASON                                                                                                                                                      |
| ----- | -------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 26912 | UTM Zone 12 N              | This is the State standard for all Utah spatial data. It has the least distortion for the state of Utah and the most accurate length and area calculations. |
| 3857  | Web Mercator               | This is the standard for all data being displayed on a web map. Most base map services are using a web mercator projection.                                 |
| 4325  | Latitude/Longitude (WGS84) | This is the ubiquitous standard for spatial data. Many systems understand this format which makes it a good for interoperability.                           |

##### Reference Information

- [What are map projections?](https://desktop.arcgis.com/en/arcmap/latest/map/projections/what-are-map-projections.htm)
- [What are projected coordinate systems?](https://desktop.arcgis.com/en/arcmap/latest/map/projections/about-projected-coordinate-systems.htm)
- [What are geographic coordinate systems?](https://desktop.arcgis.com/en/arcmap/latest/map/projections/about-geographic-coordinate-systems.htm)

#### acceptScore

Every street zone geocode will return a score for the match on a scale from 0-100. The score is a rating of how confident the system is in the choice of coordinates based on the input. For example, misspellings in a street name, omitting a street type when multiple streets with the same name exist, or omitting a street direction when the street exists in multiple quadrants will cause the result to lose points. Depending on your needs, you may need to limit the score the system will return. The default value of 70 will give acceptable results. If you need extra control, use the suggest and scoreDifference options

#### format

There are three output formats for the resulting street and zone geocoding. The default being empty. esrijson will parse into an esri.Graphic for mapping purposes and geojson will format as a a feature. If this value is omitted, the default json will be returned.

#### pobox

This option determines if the system should find a location for P.O. Box addresses. The default value of true, will return a location for a P.O. Box only when the input zone is a 5 digit zip code. The result will be where the mail is delivered. This could be a traditional post office, community post office, university, etc. When analyzing where people live, P.O. Box information will skew results since there is no correlation between where mail is delivered and where the owner of the mail liver. View the source data.

#### locators

The locators are the search engine for address data. There are three options, The default value of all will use the highest score from the address point and road center line data and provide the best match rate. Address point locations are used in the event of a tie. Address points are a work in progress with the counties to map structures or places where mail is delivered. Road centerlines are a dataset with every road and the range of numbers that road segment contains.

#### suggest

The default value of 0 will return the highest match. To include the other candidates, set this value between 1-5. The candidates respect the acceptScore option.

#### scoreDifference

When suggest is set to the default value of 0, the difference in score between the top match and the next highest match is calculated and returned on the result object. This can help determine if there was a tie. If the value is 0, repeat the request with suggest > 0 and investigate the results. A common scenario to cause a 0 is when and input address of 100 main street is input. The two highest score matches will be 100 south main and 100 north main. The system will arbitrarily choose one because they will have the same score.

#### callback

The callback function to execute for cross domain javascript calls (jsonp). This API supports CORS and does not recommend the use of callback and jsonp.

## Response Shapes

### Default Response

```json
{
  "result": {
    "location": {
      "x": 0,
      "y": 0
    },
    "score": 0,
    "locator": "",
    "matchAddress": "",
    "inputAddress": "",
    "addressGrid": ""
  },
  "status": 200
}
```

### Error Response

```json
{
  "status": 401,
  "message": ""
}
```

> Be sure to inspect the failed requests as they contain useful information. `401` status codes equates to your API key having problems. `404` equates to the address not being found. `200` is successful.
