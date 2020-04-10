import React from 'react';

export default function GettingStarted() {
  return (
    <div className="">
      <h1>Getting Started Guide</h1>
      <h2>The What?</h2>
      <p>
        The <strong>AGRC Web API</strong> is an <code>http</code> enabeled service for accessing (via the internet) the geospatial data that{' '}
        <a href="http://gis.utah.gov/about">AGRC</a> stores in the
        <a href="http://gis.utah.gov/data/how-to-connect-to-the-sgid-via-sde/" title="SGID">
          State Geographic Information Database (SGID)
        </a>
        . These services are a great way to add geospatial functionality to your web pages and applications.
      </p>
      <h2>The Who?</h2>
      <p>This guide and API are designed for people familiar with programming concepts to get started quickly and start making cool apps.</p>
      <h2>The Why?</h2>
      <p>
        The demand for <strong>geospatial</strong> and <strong>location</strong> based information has increased dramatically as many disciplines realize the
        power of spatial data. A customers address stored in a database has only so many uses. With this API, the door opens to many spatial opportunities from{' '}
        <strong>visually</strong>
        seeing customer locations on a map to being able to spatially analyze their relationship or patterns in conjunction with other events such as disease
        occurrence, natural disaster affected areas, and other location based occurrences.
      </p>
      <p>
        If there is a need to geocode addresses or find out useful information from a physical entity stored in the{' '}
        <a href="http://gis.utah.gov/data/how-to-connect-to-the-sgid-via-sde/" title="SGID">
          SGID
        </a>{' '}
        using your favorite programming language, then this API for you.
      </p>
      <h2>The How?</h2>
      <p>
        Make a <code>GET</code> or <code>POST</code> http request to our API and it will reply with the answer. The <a href="/Go/Api">API Explorer</a> lists all
        the services we currently provide. The explorer also shows you the url pattern for the service, all the required and optional parameters, what http verb
        to use, as well as a place to make a <strong>sample</strong> request. The exact url for the request will be displayed and the response will also be
        displayed. The response will be serialized as{' '}
        <a href="http://json.org" title="JSON">
          JSON
        </a>
        . In order to make a successful request, you will have to generate a unique API Key.
      </p>
      <h3>Account Creation</h3>
      <p>
        We require you to <a href="/AccountAccess">create an account</a>. Your account gives you access to <a href="/secure/KeyManagement">create and manage</a>{' '}
        your API keys. It also give you insight to how often your keys are being used. You can judge the popularity of the functionality based on these numbers.
        Be sure to
        <a href="/secure/Profile">confirm your email address</a> or API requests will not complete.
      </p>
      <h3>Key Creation</h3>
      <p>
        Once you have have an account, you can
        <a href="/secure/GenerateKey">create API keys</a>. Each key is specific to an application you have created; Either a browser or desktop based
        application.
      </p>
      <ul>
        <li>
          Browser based applications run in a web browser. eg: <a href="http://atlas.utah.gov">atlas.utah.gov</a>.{' '}
        </li>
      </ul>
      <p>
        When creating a browser key, you provide the base url for the application. In the example of
        <code>http://atlas.utah.gov</code>, the url for the key would be <code>atlas.utah.gov/*</code> which is sudo
        <a href="http://en.wikipedia.org/wiki/Regular_expression">regular expression syntax</a>. All requests that originate from the url provided will
        authenticate properly and the API will handle your request. Each key you create will track usage and you can look at your analytics in your Console.
      </p>

      <table className="table table-condensed">
        <tbody>
          <tr>
            <th>URL Pattern</th>
            <th>Description</th>
          </tr>
          <tr>
            <td>
              <code>www.example.com</code>
              <span className="block">or</span>
              <span className="block">
                <code>www.example.com/</code>
              </span>
              <span className="block">or</span>
              <span className="block">
                <code>www.example.com/*</code>
              </span>
            </td>
            <td>
              Matches all referrers in the domain <code>www.example.com</code>
            </td>
          </tr>
          <tr>
            <td>
              <code>example.com/*</code>
            </td>
            <td>
              Matches only referrers at <code>example.com</code>, but no subdomains
            </td>
          </tr>
          <tr>
            <td>
              <code>*.example.com</code>
            </td>
            <td>
              Matches all referrers at all subdomains of <code>example.com</code>
            </td>
          </tr>
          <tr>
            <td>
              <code>www.examples.com/test</code>
              <span className="block">or</span>
              <span className="block">
                <code>www.example.com/test/</code>
              </span>
              <span className="block">or</span>
              <span className="block">
                <code>www.example.com/test/*</code>
              </span>
            </td>
            <td>
              Matches all referrers in <code>www.examples.com/test/</code> and all subpaths
            </td>
          </tr>
        </tbody>
      </table>

      <p>
        It is important to note, that when developing on a local computer you create a <code>Development</code> code. localhost or machine name will only work
        with <code>Development</code> API keys.
      </p>

      <ul>
        <li>
          Desktop based applications run on a computer. eg: <code>your desktop</code>
        </li>
      </ul>

      <p>
        When creating a desktop key, you provide the ip address of that computer. For example, if I was running a python script on my local computer, I would
        generate a key with my computers ip address - lets call it
        <code>123.199.0.1</code>.
      </p>

      <h2>Wrap Up</h2>

      <ol>
        <li>You've created an account.</li>
        <li>You've confirmed your email address.</li>
        <li>You've generated a key.</li>
        <li>You've chosen what services you want to use.</li>
      </ol>

      <p>All that is left to do is choose your favorite programming language or framework and start making http requests. </p>
    </div>
  );
}
