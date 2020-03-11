const initialState = {
  street: '',
  zone: '',
  spatialReference: '26912',
  acceptScore: '70',
  pobox: false,
  locators: 'all',
  format: 'default',
  suggest: '0',
  scoreDifference: false,
  callback: ''
};

const defaultAttributes = {
  street: {
    placeholder: '123 south main street',
    type: 'string',
    required: true
  },
  zone: {
    placeholder: 'SLC or 84111',
    type: 'string',
    required: true
  },
  sr: {
    placeholder: 26912,
    type: 'int',
    required: false
  },
  acceptScore: {
    placeholder: 70,
    type: 'int',
    required: false
  },
  pobox: {
    type: 'boolean',
    required: false
  },
  locators: {
    placeholder: 'all',
    type: 'string',
    required: false
  },
  suggest: {
    placeholder: 0,
    type: 'int',
    required: false
  },
  scoreDifference: {
    placeholder: 0,
    type: 'int',
    required: false
  },
  callback: {
    placeholder: 'callback',
    type: 'string',
    required: false
  }
};

export { initialState, defaultAttributes };
