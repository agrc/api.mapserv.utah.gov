import { describe, expect, it } from 'vitest';
import { generateRegexFromPattern, getDuplicateKey } from './createKey';

describe('createKey', () => {
  it.each([null, undefined, '', '   '])('returns an empty string for %s', (value) => {
    expect(generateRegexFromPattern(value)).toEqual('');
  });
  it('generates a valid regex for a domain', () => {
    const pattern = generateRegexFromPattern('atlas.utah.gov');

    var regex = new RegExp(pattern);
    expect(pattern).toEqual(`^https?:\\/\\/atlas\\.utah\\.gov(?::\\d+)?(?:[/?#]|$)`);
    expect(regex.test('http://atlas.utah.gov/')).toBeTruthy();
    expect(regex.test('https://atlas.utah.gov/')).toBeTruthy();
  });
  it('generates a valid regex for a domain with http prefix', () => {
    const pattern = generateRegexFromPattern('http://atlas.utah.gov');

    var regex = new RegExp(pattern);
    expect(pattern).toEqual(`^https?:\\/\\/atlas\\.utah\\.gov(?::\\d+)?(?:[/?#]|$)`);
    expect(regex.test('http://atlas.utah.gov/')).toBeTruthy();
    expect(regex.test('https://atlas.utah.gov/')).toBeTruthy();
  });
  it('always creates a lowercase pattern', () => {
    const pattern = generateRegexFromPattern('HttP://atlas.Utah.GOV');

    var regex = new RegExp(pattern);
    expect(pattern).toEqual(`^https?:\\/\\/atlas\\.utah\\.gov(?::\\d+)?(?:[/?#]|$)`);
    expect(regex.test('http://atlas.utah.gov/')).toBeTruthy();
    expect(regex.test('https://atlas.utah.gov/')).toBeTruthy();
  });
  it('generates a valid regex for a domain with https prefix', () => {
    const pattern = generateRegexFromPattern('https://atlas.utah.gov');

    var regex = new RegExp(pattern);
    expect(pattern).toEqual(`^https?:\\/\\/atlas\\.utah\\.gov(?::\\d+)?(?:[/?#]|$)`);
    expect(regex.test('http://atlas.utah.gov/')).toBeTruthy();
    expect(regex.test('https://atlas.utah.gov/')).toBeTruthy();
  });
  it('generates a valid regex for a domain + slug', () => {
    const pattern = generateRegexFromPattern('atlas.utah.gov/*');

    var regex = new RegExp(pattern);
    expect(pattern).toEqual(`^https?:\\/\\/atlas\\.utah\\.gov\\/`);
    expect(regex.test('http://atlas.utah.gov/slug')).toBeTruthy();
    expect(regex.test('https://atlas.utah.gov/slug')).toBeTruthy();
  });
  it('generates a valid regex for a sub domain', () => {
    const pattern = generateRegexFromPattern('*.atlas.utah.gov');

    var regex = new RegExp(pattern);
    expect(pattern).toEqual(`^https?:\\/\\/[a-z0-9._-]+\\.atlas\\.utah\\.gov(?::\\d+)?(?:[/?#]|$)`);
    expect(regex.test('http://sub.atlas.utah.gov/')).toBeTruthy();
    expect(regex.test('https://sub.atlas.utah.gov/')).toBeTruthy();
  });
  it.each([
    ['www.example.com', 'https://www.example.com/', true],
    ['www.example.com', 'http://www.example.com/index.html', true],
    ['www.example.com', 'http://www.example.com/request/test.html', true],
    ['www.example.com', 'http://www.example.com/request/test/index.html?query=yes', true],

    ['www.example.com', 'http://www.badexample.com/', false],
    ['www.example.com', 'http://www.badexample.com/index.html', false],
    ['www.example.com', 'http://www.badexample.com/request/test.html', false],
    ['www.example.com', 'http://www.badexample.com/request/test/index.html?query=yes', false],

    ['www.example.com/*', 'http://www.example.com/', true],
    ['www.example.com/*', 'http://www.example.com/index.html', true],
    ['www.example.com/*', 'http://www.example.com/reqes/test.html', true],
    ['www.example.com/*', 'http://www.example.com/request/test/index.html?query=yes', true],

    ['www.example.com/*', 'http://www.badexample.com/', false],
    ['www.example.com/*', 'http://www.badexample.com/index.html', false],
    ['www.example.com/*', 'http://www.badexample.com/request/test.html', false],
    ['www.example.com/*', 'http://www.badexample.com/request/test/index.html?query=yes', false],

    ['www.example.com/', 'http://www.example.com/', true],
    ['www.example.com/', 'http://www.example.com/index.html', true],

    ['example.com/*', 'http://example.com/index.html', true],
    ['example.com/*', 'http://example.com/request/index.html', true],

    ['example.com/*', 'http://bad.example.com/index.html', false],
    ['example.com/*', 'http://bad.example.com/request/index.html', false],

    ['*.example.com', 'http://any.example.com/', true],
    ['*.example.com', 'http://any.example.com/index.html', true],
    ['*.example.com', 'http://any.example.com/request/test.html', true],
    ['*.example.com', 'http://any.example.com/request/test/index.html?query=yes', true],

    ['www.example.com/test', 'http://www.example.com/test/index.html', true],
    ['www.example.com/test', 'http://www.example.com/test', true],

    ['www.example.com/test', 'http://www.example.com/bad', false],
    ['www.example.com/test', 'http://www.example.com/bad/index.html', false],
    ['www.example.com/test', 'http://bad.example.com/test/index.html', false],

    ['www.example.com/test/*', 'http://www.example.com/test/index.html', true],
    ['www.example.com/test/*', 'http://www.example.com/test/test2/index.html', true],

    ['www.example.com/test/*', 'http://bad.example.com/test/test/index.html', false],
    ['www.example.com/test/*', 'http://www.example.com/bad/test2/index.htm', false],

    ['*.nedds.health.utah.gov*', 'http://www.nedds.health.utah.gov', true],
    ['api.utlegislators.com', 'http://api.utlegislators.com', true],
    ['sub.domain:8080', 'https://sub.domain:8080', true],

    // only a leading scheme is stripped so a scheme later in the path is matched literally
    ['example.com/http://evil.net', 'https://example.com/http://evil.net', true],
    ['example.com/http://evil.net', 'https://example.com/evil.net', false],

    // the host must end where the pattern ends so a look-alike domain can not borrow the key
    ['example.com', 'https://example.com.evil.net/', false],
    ['example.com', 'https://example.community/', false],
    ['example.com', 'https://evil.net/example.com/', false],
    ['example.com', 'https://example.com:8443/map?x=1', true],
    ['example.com', 'https://example.com?x=1', true],
    ['example.com', 'https://example.com#top', true],
    ['sub.domain:8080', 'https://sub.domain:80800/', false],
    ['sub.domain:8080', 'https://sub.domain/', false],

    // the subdomain wildcard can not cross into the port, path or query
    ['*.example.com', 'https://a.b.example.com/', true],
    ['*.example.com', 'https://example.com/', false],
    ['*.example.com', 'https://a.example.com.evil.net/', false],
    ['*.example.com', 'https://evil.net/x.example.com', false],
    ['*.example.com', 'https://evil.net/?x=a.example.com', false],
    ['*.example.com/*', 'https://x.example.com/anything', true],
    ['*.example.com/*', 'https://evil.net/x.example.com/', false],
    ['*.nedds.health.utah.gov*', 'http://www.nedds.health.utah.gov.evil.net/', false],
    ['example.com*', 'https://example.com/anything', true],
    ['example.com*', 'https://example.com.evil.net/', false],

    // regex metacharacters in the path are matched literally
    ['example.com/app?x=1', 'https://example.com/app?x=1', true],
    ['example.com/app?x=1', 'https://example.com/ap', false],
    ['example.com/a.b-c_d', 'https://example.com/a.b-c_d/index.html', true],
    ['example.com/a.b-c_d', 'https://example.com/aXb-c_d/index.html', false],
  ])('user pattern %s with %s is %s', (input, url, expected) => {
    const pattern = generateRegexFromPattern(input);

    var regex = new RegExp(pattern);
    expect(regex.test(url)).toEqual(expected);
  });

  it('matches regex metacharacters in the path literally', () => {
    const pattern = generateRegexFromPattern('example.com/(a+)+$');

    expect(pattern).toEqual(`^https?:\\/\\/example\\.com\\/\\(a\\+\\)\\+\\$`);

    const regex = new RegExp(pattern);
    expect(regex.test('https://example.com/(a+)+$')).toBeTruthy();
    expect(regex.test('https://example.com/aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa/')).toBeFalsy();
  });
  it.each([
    '(a+)+$',
    '(.*)',
    '.*',
    'example.com|evil.net',
    'exa(m)ple.com',
    'example.com?',
    'foo.*.com',
    'exam*ple.com',
    'example.com/*/map',
    '**.example.com',
    '*.',
    '/app',
    '-example.com',
    'exa mple.com',
    'example.com:',
    '[::1]:3000',
    '*168.177.222.22/app/*',
    '*example.com',
    '155.100.*',
    '155.100.*.*',
    'example.com.',
    'example-',
  ])('rejects %s because it is not a host name with optional wildcards', (input) => {
    expect(generateRegexFromPattern(input)).toEqual('');
  });
});

const browserKey = {
  for: 'me',
  pattern: 'atlas.utah.gov',
  ip: '',
  type: 'browser',
  mode: 'development',
  notes: 'notes are ignored',
};
const serverKey = {
  for: 'me',
  pattern: '',
  ip: '0.0.0.0',
  type: 'server',
  mode: 'development',
  notes: 'notes are ignored',
};

describe('getDuplicateKey', () => {
  it('returns null when user has no existing keys', () => {
    expect(getDuplicateKey([], browserKey)).toEqual(null);
    expect(getDuplicateKey([{}], browserKey)).toEqual(null);
    expect(getDuplicateKey(null, browserKey)).toEqual(null);
    expect(getDuplicateKey(undefined, browserKey)).toEqual(null);
    expect(getDuplicateKey('', browserKey)).toEqual(null);
    expect(getDuplicateKey(1, browserKey)).toEqual(null);
  });
  it('returns null when mode is different', () => {
    const keys = [
      {
        flags: { production: true, server: false },
        pattern: 'atlas.utah.gov',
      },
      {
        flags: { production: true, server: true },
        pattern: '0.0.0.0',
      },
    ];

    expect(getDuplicateKey(keys, browserKey)).toEqual(null);
    expect(getDuplicateKey(keys, serverKey)).toEqual(null);
  });
  it('returns a key when pattern matches', () => {
    const keys = [
      {
        flags: { production: true, server: false },
        pattern: 'atlas.utah.gov',
        key: 'duplicateBrowser',
      },
      {
        flags: { production: true, server: true },
        pattern: '0.0.0.0',
        key: 'duplicateServer',
      },
    ];

    const duplicateBrowserKey = {
      for: 'me',
      pattern: 'atlas.utah.gov',
      ip: '',
      type: 'browser',
      mode: 'production',
      notes: 'notes are ignored',
    };

    const duplicateServerKey = {
      for: 'me',
      type: 'server',
      ip: '0.0.0.0',
      mode: 'production',
      notes: 'notes are ignored',
    };

    expect(getDuplicateKey(keys, duplicateBrowserKey)).toEqual('duplicateBrowser');
    expect(getDuplicateKey(keys, duplicateServerKey)).toEqual('duplicateServer');
  });
});
