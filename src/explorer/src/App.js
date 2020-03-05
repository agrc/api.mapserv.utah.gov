import React from 'react';
import "./App.css";
import { TopNav, SideNav } from './components/Navigation';
import Header from './components/Header';
import { StreetZoneEndpoint } from './components/Endpoint';

export default function App() {
  return (
    <>
      <TopNav></TopNav>
      <main className="flex h-screen pt-5">
        <SideNav></SideNav>
        <section className="flex-auto overflow-auto">
          <main className="h-full min-h-screen max-w-screen-xl mx-auto md:px-4 lg:px-8">
            <Header></Header>
            <section className="bg-pattern mt-12 bg-gray-300 h-full py-2">
              <div className="ml-1 bg-white border-l-2 border-indigo-300 rounded-l rounded-r md:-mr-1 border-r-8 border-t-2 border-b-2">
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
    </>
  );
}
