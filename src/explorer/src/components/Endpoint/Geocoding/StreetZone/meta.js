import { object, string, number, boolean, addMethod, mixed } from 'yup';

const schema = object().shape({
  street: string().required().meta({ placeholder: '123 south main street'}),
  zone: string().required().meta({ placeholder: 'SLC or 84111'}),
  spatialReference: number().positive().integer().default(26912),
  acceptScore: number().positive().integer().default(70),
  pobox: boolean().default(false),
  locators: string().oneOf(['all', 'addressPoints', 'roadCenterlines']).default('all'),
  format: string().oneOf(['default', 'esrijson', 'geojson']).default('default'),
  suggest: number().positive().integer().default(0),
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
