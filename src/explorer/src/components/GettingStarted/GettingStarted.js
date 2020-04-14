import React from 'react';
import CallToAction from '../Action';
import { Link, H2, P, BrowserKeyExamples, Card, CardItem } from './Elements';

export default function GettingStarted() {
  return (
    <article className="text-gray-700">
      <div className="bg-pattern bg-topo p-6 border-b border-gray-400 shadow-xs">
        <h1 className="md:text-5xl text-3xl text-gray-800 leading-tight font-light font-extrabold tracking-tight">
          Getting <span className="text-indigo-600">started</span>
        </h1>
        <h2 className="md:text-3xl text-2xl block text-gray-700 font-light tracking-tight">
          Your first <span className="font-semibold">successful</span> request in <span className="text-indigo-600 font-semibold">3</span> steps.
        </h2>
        <section className="px-6 max-w-5xl mx-auto pt-6 ">
          <div className="mt-10">
            <ul className="md:grid md:grid-cols-2 md:col-gap-8 md:row-gap-10">
              <li>
                <div className="flex">
                  <div className="flex-shrink-0">
                    <div className="flex items-center justify-center h-12 w-12 rounded-md bg-indigo-500 text-white text-3xl font-black">1</div>
                  </div>
                  <div className="ml-4">
                    <h5 className="text-lg leading-6 font-medium">Create a complimentary account</h5>
                    <p className="mt-2 text-base leading-6 text-gray-500">Manage and view analytics for all of your API keys.</p>
                  </div>
                </div>
              </li>
              <li className="mt-10 md:mt-0">
                <div className="flex">
                  <div className="flex-shrink-0">
                    <div className="flex items-center justify-center h-12 w-12 rounded-md bg-indigo-500 text-white text-3xl font-black">2</div>
                  </div>
                  <div className="ml-4">
                    <h5 className="text-lg leading-6 font-medium">Confirm your email</h5>
                    <p className="mt-2 text-base leading-6 text-gray-500">Control the bots.</p>
                  </div>
                </div>
              </li>
              <li className="mt-10 md:mt-0">
                <div className="flex">
                  <div className="flex-shrink-0">
                    <div className="flex items-center justify-center h-12 w-12 rounded-md bg-indigo-500 text-white text-3xl font-black">3</div>
                  </div>
                  <div className="ml-4">
                    <h5 className="text-lg leading-6 font-medium">Generate an API key</h5>
                    <p className="mt-2 text-base leading-6 text-gray-500">Keep track of usage analytics and authenticate requests.</p>
                  </div>
                </div>
              </li>
              <li className="mt-10 md:mt-0">
                <div className="flex">
                  <div className="flex-shrink-0">
                    <div className="flex items-center justify-center h-12 w-12 rounded-md bg-indigo-500 text-white text-3xl font-black">4</div>
                  </div>
                  <div className="ml-4">
                    <h5 className="text-lg leading-6 font-medium">Make a request</h5>
                    <p className="mt-2 text-base leading-6 text-gray-500">Using your favorite programming language or one of the AGRC tools.</p>
                  </div>
                </div>
              </li>
            </ul>
          </div>
        </section>
        <CallToAction></CallToAction>
      </div>
      {/* <aside className="px-6 max-w-5xl mx-auto pt-6 mb-10">
        <div>
          <H2>Location Matters</H2>
          <P>
            The demand for <span className="font-semibold">geospatial</span> and <span className="font-semibold">location</span> based information has increased dramatically as many disciplines realize
            the power of spatial data. A customer's address stored in a database has only so many uses. With this API, the door opens to many spatial
            opportunities from <span className="font-semibold">visually</span> seeing customer locations on a map to being able to spatially analyze their relationship or patterns in
            conjunction with other events such as disease occurrences, natural disaster affected areas, and other location based occurrences.
          </P>
        </div>
      </aside> */}
      <div className="p-6 border-b border-gray-400 shadow-xs">
        <section className="px-6 max-w-5xl mx-auto">
          <H2>Key Creation</H2>
          <P>
            Once you have have an account and confirmed ownership of your email address, you can{' '}
            <a href="https://developer.mapserv.utah.gov/secure/GenerateKey">generate API keys</a>. Each key is specific to an application you have created;
            Either a browser or desktop based application. Browser based applications run in a web browser. eg: the javascript geocoding component dartboard,
            running on atlas.utah.gov. The request to the Web API is created in javascript running inside the browser using the browsers fetch API or with an
            XHR request. Desktop based applications run on a computer. eg: the ArcGIS Pro Geocoding tool running on your desktop. The request to the Web API is
            coded directly or indirectly from a server side programming or scripting language like Python, Java, or C#.
          </P>
        </section>
      </div>
      <div className="border-b border-gray-400 shadow-xs">
        <section className="w-full relative">
          <div className="bg-indigo-800 h-64 absolute inset-0"></div>
          <section className="px-6 max-w-5xl mx-auto z-10 relative">
            <H2 className="pt-3 md:col-span-2 text-white text-center">Choosing the key type</H2>
            <div className="md:grid md:grid-cols-2 md:col-gap-10">
              <Card type="Browser" tagLine="Requests are made by JavaScript running in a browser">
                <CardItem>XHR request</CardItem>
                <CardItem>window.fetch</CardItem>
                <CardItem>jquery.ajax</CardItem>
                <CardItem>esri js api widgets</CardItem>
                <CardItem>dojo widgets</CardItem>
                <CardItem>react components</CardItem>
                <CardItem>angular components</CardItem>
              </Card>
              <Card className="mt-6 md:mt-0" type="Desktop" tagLine="Requests are made by code or a tool executing on your computer or a server">
                <CardItem>ArcPy</CardItem>
                <CardItem>ArcGIS Desktop</CardItem>
                <CardItem>Python</CardItem>
                <CardItem>Java</CardItem>
                <CardItem>C#</CardItem>
                <CardItem>AGRC Geocoding Toolbox</CardItem>
              </Card>
            </div>
          </section>
        </section>

        <section className="max-w-5xl mt-6 md:mx-auto md:grid md:grid-cols-2 md:col-gap-5">
          <H2 className="px-6 col-span-2">Choosing a browser key</H2>
          <div class="md:col-span-2 text-center flex mb-6">
            <div class="p-2 bg-indigo-800 items-center text-indigo-100 lg:rounded-full flex lg:inline-flex" role="alert">
              <span class="flex rounded-full bg-indigo-500 uppercase px-2 py-1 text-xs font-bold mr-3">tip</span>
              <span class="font-semibold mr-2 text-left flex-auto">
                During local development of browser based applications, the Application Status of the key needs to be set to{' '}
                <span className="font-semibold">Development</span> for requests originating from localhost or machine name urls.
              </span>
            </div>
          </div>
          <div className="px-6 max-w-5xl mx-auto">
            <P>
              The AGRC Web API collects analytics and authenticates requests with your browser key. Since your application is running in a browser, the API key
              is in public view. To ensure that the analytics are accurate, the key that you create for the application will contain the url the requests
              originate from.
            </P>
            <P className="mt-6">
              For example, if your react application is running on atlas.utah.gov, your key pattern will simply equal the DNS name. If your ember application is
              running on gis.utah.gov/my-application, then your key will also equal the URL.
            </P>
            <P className="mt-6">
              To get the URL pattern correct when creating the key, reference the table for examples. Be sure to always read the response body when making
              requests to an endpoint. Failed requests will provide helpful information to correct the issue
            </P>
          </div>
          <BrowserKeyExamples className="mx-6 mt-6 md:mt-0 md:max-w-5xl md:mr-6"></BrowserKeyExamples>
        </section>
        <section className="max-w-5xl mx-auto mt-6">
          <H2 className="px-6 col-span-2">Choosing a desktop key</H2>
          <div class="md:col-span-2 text-center lg:px-4 flex mb-6">
            <div class="p-2 bg-indigo-800 items-center text-indigo-100 lg:rounded-full flex lg:inline-flex" role="alert">
              <span class="flex rounded-full bg-indigo-500 uppercase px-2 py-1 text-xs font-bold mr-3">tip</span>
              <span class="font-semibold mr-2 text-left flex-auto">
                A common mistake is to use your local IP address. Check with a website like <Link to="https://www.whatismyip.net/">what is my ip</Link> to see
                what your public IP address is.
              </span>
            </div>
          </div>
          <div className="px-6 max-w-5xl mx-auto">
            <P>
              The AGRC Web API collects analytics and authenticates requests with your desktop key. To ensure that the analytics are accurate, the key that you
              create for the application will contain the IP address the requests originate from.
            </P>
            <P className="mt-6">
              For example, if you are using the AGRC Geocoding Tool in ArcGIS Pro, the requests the tool creates will originate from the IP address assigned to
              you from your internet service provider. This address is not always static and multiple keys may need to be created over time.
            </P>
            <P className="mt-6">
              Be sure to always read the response body when making requests to an endpoint. Failed requests will provide helpful information to correct the
              issue
            </P>
          </div>
        </section>
      </div>
    </article>
  );
}
