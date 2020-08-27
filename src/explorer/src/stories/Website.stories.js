import React from 'react';
import Action from '../components/Action';
import { ResponsiveSideNav, InlineTopNav } from '../components/Navigation';
import Button from '../components/Button';
import { BrowserRouter as Router } from 'react-router-dom';

export default {
  title: 'Website Parts'
};

export const CallToAction = () => <Action></Action>;
export const SideNavigation = () => <Router><ResponsiveSideNav></ResponsiveSideNav></Router>;
export const TopNavigation = () => <Router><InlineTopNav></InlineTopNav></Router>;
export const Buttons = () => (
  <>
    <Button>normal</Button>
    <Button disabled>disabled</Button>
  </>
);
