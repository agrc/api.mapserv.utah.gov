/* eslint-disable no-useless-escape */
import { describe, expect, it } from 'vitest';
import { generateRegexFromPattern } from './createKey';

describe('createKey', () => {
  it.each([null, undefined, '', '   '])(
    'returns an empty string for %s',
    (value) => {
      expect(generateRegexFromPattern(value)).toEqual('');
    },
  );
  it('generates a valid regex for a domain', () => {
    const pattern = generateRegexFromPattern('atlas.utah.gov');

    var regex = new RegExp(pattern);
    expect(pattern).toEqual(`^https?://atlas\\.utah\\.gov`);
    expect(regex.test('http://atlas.utah.gov/')).toBeTruthy();
    expect(regex.test('https://atlas.utah.gov/')).toBeTruthy();
  });
  it('generates a valid regex for a domain with http prefix', () => {
    const pattern = generateRegexFromPattern('http://atlas.utah.gov');

    var regex = new RegExp(pattern);
    expect(pattern).toEqual(`^https?://atlas\\.utah\\.gov`);
    expect(regex.test('http://atlas.utah.gov/')).toBeTruthy();
    expect(regex.test('https://atlas.utah.gov/')).toBeTruthy();
  });
  it('always creates a lowercase pattern', () => {
    const pattern = generateRegexFromPattern('HttP://atlas.Utah.GOV');

    var regex = new RegExp(pattern);
    expect(pattern).toEqual(`^https?://atlas\\.utah\\.gov`);
    expect(regex.test('http://atlas.utah.gov/')).toBeTruthy();
    expect(regex.test('https://atlas.utah.gov/')).toBeTruthy();
  });
  it('generates a valid regex for a domain with https prefix', () => {
    const pattern = generateRegexFromPattern('https://atlas.utah.gov');

    var regex = new RegExp(pattern);
    expect(pattern).toEqual(`^https?://atlas\\.utah\\.gov`);
    expect(regex.test('http://atlas.utah.gov/')).toBeTruthy();
    expect(regex.test('https://atlas.utah.gov/')).toBeTruthy();
  });
  it('generates a valid regex for a domain + slug', () => {
    const pattern = generateRegexFromPattern('atlas.utah.gov/*');

    var regex = new RegExp(pattern);
    expect(pattern).toEqual(`^https?://atlas\\.utah\\.gov/.*`);
    expect(regex.test('http://atlas.utah.gov/slug')).toBeTruthy();
    expect(regex.test('https://atlas.utah.gov/slug')).toBeTruthy();
  });
  it('generates a valid regex for a sub domain', () => {
    const pattern = generateRegexFromPattern('*.atlas.utah.gov');

    var regex = new RegExp(pattern);
    expect(pattern).toEqual(`^https?://.+\\.atlas\\.utah\\.gov`);
    expect(regex.test('http://sub.atlas.utah.gov/')).toBeTruthy();
    expect(regex.test('https://sub.atlas.utah.gov/')).toBeTruthy();
  });
  it.each([
    ['www.example.com', 'https://www.example.com/', true],
    ['www.example.com', 'http://www.example.com/index.html', true],
    ['www.example.com', 'http://www.example.com/request/test.html', true],
    [
      'www.example.com',
      'http://www.example.com/request/test/index.html?query=yes',
      true,
    ],

    ['www.example.com', 'http://www.badexample.com/', false],
    ['www.example.com', 'http://www.badexample.com/index.html', false],
    ['www.example.com', 'http://www.badexample.com/request/test.html', false],
    [
      'www.example.com',
      'http://www.badexample.com/request/test/index.html?query=yes',
      false,
    ],

    ['www.example.com/*', 'http://www.example.com/', true],
    ['www.example.com/*', 'http://www.example.com/index.html', true],
    ['www.example.com/*', 'http://www.example.com/reqes/test.html', true],
    [
      'www.example.com/*',
      'http://www.example.com/request/test/index.html?query=yes',
      true,
    ],

    ['www.example.com/*', 'http://www.badexample.com/', false],
    ['www.example.com/*', 'http://www.badexample.com/index.html', false],
    ['www.example.com/*', 'http://www.badexample.com/request/test.html', false],
    [
      'www.example.com/*',
      'http://www.badexample.com/request/test/index.html?query=yes',
      false,
    ],

    ['www.example.com/', 'http://www.example.com/', true],
    ['www.example.com/', 'http://www.example.com/index.html', true],

    ['example.com/*', 'http://example.com/index.html', true],
    ['example.com/*', 'http://example.com/request/index.html', true],

    ['example.com/*', 'http://bad.example.com/index.html', false],
    ['example.com/*', 'http://bad.example.com/request/index.html', false],

    ['*.example.com', 'http://any.example.com/', true],
    ['*.example.com', 'http://any.example.com/index.html', true],
    ['*.example.com', 'http://any.example.com/request/test.html', true],
    [
      '*.example.com',
      'http://any.example.com/request/test/index.html?query=yes',
      true,
    ],

    ['www.example.com/test', 'http://www.example.com/test/index.html', true],
    ['www.example.com/test', 'http://www.example.com/test', true],

    ['www.example.com/test', 'http://www.example.com/bad', false],
    ['www.example.com/test', 'http://www.example.com/bad/index.html', false],
    ['www.example.com/test', 'http://bad.example.com/test/index.html', false],

    ['www.example.com/test/*', 'http://www.example.com/test/index.html', true],
    [
      'www.example.com/test/*',
      'http://www.example.com/test/test2/index.html',
      true,
    ],

    [
      'www.example.com/test/*',
      'http://bad.example.com/test/test/index.html',
      false,
    ],
    [
      'www.example.com/test/*',
      'http://www.example.com/bad/test2/index.htm',
      false,
    ],

    ['*.nedds.health.utah.gov*', 'http://www.nedds.health.utah.gov', true],
    ['api.utlegislators.com', 'http://api.utlegislators.com', true],
    ['*168.177.222.22/app/*', 'http://168.177.222.22/app/whatever', true],
    ['sub.domain:8080', 'https://sub.domain:8080', true],
  ])('user pattern %s with %s is %s', (input, url, expected) => {
    const pattern = generateRegexFromPattern(input);

    var regex = new RegExp(pattern);
    expect(regex.test(url)).toEqual(expected);
  });
});
