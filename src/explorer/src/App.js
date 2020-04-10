import React from 'react';
import { TopNav, SideNav } from './components/Navigation';
import Landing from './components/Home/Landing';
import SellIt from './components/Home/Sellit';
import Whats from './components/Home/Whats';
import Goals from './components/Home/Goals';
import GettingStarted from './components/GettingStarted';
import { StreetZoneEndpoint } from './components/Endpoint/Geocoding';
import { BrowserRouter as Router, Switch, Route } from 'react-router-dom';

export default function App() {
  return (
    <Router>
      <Switch>
        <Route exact path="/">
          <div>
            <div className="bg-pattern bg-topo p-6 border-b border-gray-400 shadow-xs">
              <Landing></Landing>
            </div>
            <div className="max-w-5xl mx-auto pt-5 px-6">
              <SellIt></SellIt>
            </div>
            <div className="flex">
              <div className="hidden md:block flex-none w-40 sticky top-0 self-start pt-6 ml-6">
                <SideNav></SideNav>
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
          </div>
        </Route>
        <Route exact path="/getting-started">
          <GettingStarted></GettingStarted>
        </Route>
        <Route exact path="/documentation">
          <main className="flex h-screen">
            <div className="hidden md:block flex-none w-40 sticky top-0 self-start pt-6 ml-6">
              <SideNav></SideNav>
            </div>
            <section className="flex-auto overflow-auto">
              <main className="h-full min-h-screen max-w-screen-xl mx-auto md:px-4 lg:px-8">
                <section className="bg-pattern bg-circuit border border-gray-400 mt-12 h-screen py-2">
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
          <TopNav></TopNav>
        </Route>
      </Switch>
    </Router>
  );
}
