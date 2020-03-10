import stringify, { hasRequiredParts, getRequiredParts } from './QueryString';

test('gets required parts', () => {
  const parts = getRequiredParts('/:these/:are/:required/this/is/not');

  expect(parts.length).toEqual(3);
  expect(parts).toEqual(['these', 'are', 'required']);
});

test('returns false when required parts have no values', () => {
  const initialState = { required: '' };

  expect(hasRequiredParts({}, '/:required', initialState)).toBeFalsy();
  expect(hasRequiredParts(null, '/:required', initialState)).toBeFalsy();
  expect(hasRequiredParts(undefined, '/:required', initialState)).toBeFalsy();
});

test('returns false when required parts equal the default value', () => {
  const initialState = { required: '' };

  expect(hasRequiredParts({ required: '' }, '/:required', initialState)).toBeFalsy();
});

test('returns true when required parts have value', () => {
  const initialState = { required: '', alsoRequired: null };

  expect(hasRequiredParts({ required: true }, '/:required', initialState)).toBeTruthy();
  expect(hasRequiredParts({ required: 1, alsoRequired: '' }, '/:required/notRequired/:alsoRequired', initialState)).toBeTruthy();
  expect(hasRequiredParts({ required: 'yes', other: 'no effect' }, '/:required', initialState)).toBeTruthy();
});

test('replaces required parts with values', () => {
  const initialState = { required: false, alsoRequired: 0 };

  expect(stringify({ required: true, alsoRequired: 1 }, '/not-required/:required/:alsoRequired/extra', initialState)).toEqual('/not-required/true/1/extra');
});

test('append extra properties as querystring values', () => {
  const initialState = { required: false, optional: '' };

  expect(stringify({ required: true, optional: 'yes' }, '/:required/part', initialState)).toEqual('/true/part?optional=yes');
});

test('does not stringify optional parameters if they are default values', () => {
  const initialState = { required: false, optional: 'yes' };
  expect(stringify({ required: true, optional: 'yes' }, '/:required/part', initialState)).toEqual('/true/part');
});
