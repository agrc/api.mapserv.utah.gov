import { object, string, number, boolean, addMethod, mixed } from 'yup';

const schema = object().shape({
  street: string().matches(/^\d*\s\D/, {
    message: 'this must be a valid street address',
    excludeEmptyString: true
  }).required().meta({ placeholder: '123 south main street'}),
  zone: string().matches(/(^84\d{3}$|^\D)/, {
      message: 'this must be a valid Utah zip code or city name',
      excludeEmptyString: true
    }).required().meta({ placeholder: 'SLC or 84111' }),
  // https://enterprise.arcgis.com/en/sdk/latest/windows/IGeometryServer_FindSRByWKID.html
  spatialReference: number().min(1000).max(209199).integer().default(26912),
  acceptScore: number().min(0).max(100).integer().default(70),
  pobox: boolean().default(false),
  locators: string().oneOf(['all', 'addressPoints', 'roadCenterlines']).default('all'),
  format: string().oneOf(['default', 'esrijson', 'geojson']).default('default'),
  suggest: number().min(0).max(5).integer().default(0),
  scoreDifference: boolean().default(false),
  callback: string().default('callback')
});

// add some custom convenience methods to help reduce code
addMethod(mixed, 'isFieldRequired', function() {
  // it's not very easy to figure out if a field is required in yup
  return this.describe().tests.findIndex(({ name }) => name === 'required') >= 0;
});
addMethod(mixed, 'getPlaceholder', function() {
  return (this.meta()) ? this.meta().placeholder : null;
});

export default schema;
