import React from 'react';
import ScrollToTop from './components/ScrollToTop';
import { ResponsiveSideNav, FixedBottomNav, InlineTopNav, CommonLinks } from './components/Navigation';
import { Landing, SellIt, Whats, Goals } from './components/Home';
import GettingStarted from './components/GettingStarted';
import { StreetZoneEndpoint } from './components/Endpoint/Geocoding';
import { BrowserRouter as Router, Switch, Route, Redirect } from 'react-router-dom';
import PrivacyPolicy from './components/PrivacyPolicy';
import ReverseEndpoint from './components/Endpoint/Geocoding/Reverse/ReverseEndpoint';
import SearchEndpoint from './components/Endpoint/Searching/Search/SearchEndpoint';
import EndpointCategory from './components/Endpoint/EndpointCategory';

const DOCUMENTATION_PATH = 'documentation';
const DEFAULT_API_VERSION = 'v2';

export default function App() {
  return (
    <Router>
      <ScrollToTop />
      <Switch>
        <Route exact path="/">
          <div className="bg-pattern bg-topo p-6 border-b border-gray-400 shadow-xs">
            <Landing></Landing>
          </div>
          <div className="max-w-5xl mx-auto pt-5 px-6">
            <SellIt></SellIt>
          </div>
          <div className="flex">
            <div className="hidden md:block flex-none w-40 sticky top-0 self-start pt-6 ml-6">
              <ResponsiveSideNav></ResponsiveSideNav>
            </div>
            <div className="md:pt-4 px-6">
              <div className="pb-6">
                <Whats></Whats>
              </div>
            </div>
          </div>
          <div className="bg-pattern bg-topo p-6 border border-gray-400 shadow-xs">
            <Goals></Goals>
          </div>
          <FixedBottomNav>
            <CommonLinks></CommonLinks>
          </FixedBottomNav>
        </Route>
        <Route exact path="/getting-started">
          <InlineTopNav>
            <CommonLinks></CommonLinks>
          </InlineTopNav>
          <GettingStarted></GettingStarted>
        </Route>
        <Route exact path={`/${DOCUMENTATION_PATH}`}>
          <Redirect to={`/${DOCUMENTATION_PATH}/${DEFAULT_API_VERSION}`} />
        </Route>
        <Route exact path={`/${DOCUMENTATION_PATH}/:apiVersion/:endpointCategory?/:endpoint?/:display?`}>
          <InlineTopNav>
            <CommonLinks></CommonLinks>
          </InlineTopNav>
          <main className="flex">
            <div className="hidden md:block flex-none w-40 sticky top-0 self-start pt-6 ml-6">
              <ResponsiveSideNav></ResponsiveSideNav>
            </div>
            <section className="w-full">
              <section className="bg-gray-300 bg-circuit--light border border-gray-400 py-2 h-full">
                <EndpointCategory id="geocoding"
                  title="Geocoding Endpoints" subtitle="Everything related to addresses">
                  <StreetZoneEndpoint />
                  <ReverseEndpoint />
                </EndpointCategory>
                <EndpointCategory id="searching"
                  title="Searching Endpoints" subtitle="API methods for searching SGID data">
                  <SearchEndpoint />
                </EndpointCategory>
              </section>
            </section>
          </main>
        </Route>
        <Route exact path="/privacy-policy">
          <InlineTopNav>
            <CommonLinks></CommonLinks>
          </InlineTopNav>
          <PrivacyPolicy></PrivacyPolicy>
        </Route>
      </Switch>
    </Router>
  );
}
