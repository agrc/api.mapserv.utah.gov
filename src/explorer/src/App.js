import React from 'react';
import ScrollToTop from './components/ScrollToTop';
import { ResponsiveSideNav, FixedBottomNav, InlineTopNav, CommonLinks } from './components/Navigation';
import { Landing, SellIt, Whats, Goals } from './components/Home';
import GettingStarted from './components/GettingStarted';
import { StreetZoneEndpoint } from './components/Endpoint/Geocoding';
import { BrowserRouter as Router, Switch, Route } from 'react-router-dom';
import PrivacyPolicy from './components/PrivacyPolicy';

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
        <Route exact path="/documentation">
          <InlineTopNav>
            <CommonLinks></CommonLinks>
          </InlineTopNav>
          <main className="flex h-screen">
            <div className="hidden md:block flex-none w-40 sticky top-0 self-start pt-6 ml-6">
              <ResponsiveSideNav></ResponsiveSideNav>
            </div>
            <section className="flex-auto overflow-auto">
              <main className="h-full min-h-screen max-w-screen-xl mx-auto md:px-4 lg:px-8">
                <section className="bg-gray-300 bg-circuit--light border border-gray-400 mt-12 h-screen py-2">
                  <div
                    id="geocoding"
                    className="ml-1 bg-white border-l-2 border-indigo-300 md:rounded-l md:rounded-r md:-mr-1 border-r-8 border-t-2 border-b-2">
                    <header className="p-3">
                      <h3 className="text-3xl font-extrabold tracking-tight">Geocoding Endpoints</h3>
                      <p className="text-sm text-gray-500 tracking-wider">Everything related to addresses</p>
                    </header>
                  </div>
                  <StreetZoneEndpoint></StreetZoneEndpoint>
                </section>
              </main>
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
