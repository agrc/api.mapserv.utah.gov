import React from 'react';
import Header from '../components/Header';
import Action from '../components/Action';
import { SideNav, TopNav } from '../components/Navigation';

export default {
  title: 'Website Parts'
};

export const Headers = () => <Header></Header>;
export const CallToAction = () => <Action></Action>;
export const SideNavigation = () => <SideNav></SideNav>;
export const TopNavigation = () => <TopNav></TopNav>;
