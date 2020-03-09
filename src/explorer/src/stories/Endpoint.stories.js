import React from 'react';
import Endpoint from '../components/Endpoint/Endpoint';
import EndpointDemoDocToggle from '../components/Endpoint/EndpointDemoDocToggle';
import EndpointResponseFormat from '../components/Endpoint/EndpointResponseFormat';
import EndpointAdvancedToggle from '../components/Endpoint/EndpointAdvancedToggle';
import EndpointInput from '../components/Endpoint/EndpointInput';
import { TypeLabel, RequiredLabel } from '../components/Endpoint/EndpointLabel';
import EndpointSelect from '../components/Endpoint/EndpointSelect';
import EndpointSwitch from '../components/Endpoint/EndpointSwitch';

export default {
  title: 'Endpoint Parts',
  component: Endpoint
};

export const DemoDocToggle = () => <EndpointDemoDocToggle active='docs' showApi={() => {}}></EndpointDemoDocToggle>;
export const ResponseFormat = () => <EndpointResponseFormat></EndpointResponseFormat>;
export const AdvancedToggle = () => <EndpointAdvancedToggle></EndpointAdvancedToggle>;
export const Input = () => <EndpointInput type="int" placeholder="placeholder" required={true}></EndpointInput>;
export const Label = () => (<><TypeLabel type="string"></TypeLabel><RequiredLabel required={false}></RequiredLabel></>);
export const Select = () => <EndpointSelect type="int" required={false}><option>options</option></EndpointSelect>;
export const Switch = () => <EndpointSwitch type="boolean" required={true}></EndpointSwitch>;
