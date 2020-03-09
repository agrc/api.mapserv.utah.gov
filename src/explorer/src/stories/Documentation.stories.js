import React from 'react';
import { Tip, Code, TipLink, Link, Label, Description, Heading, SubHeading } from '../components/Endpoint/Documentation/Elements';

export default {
  title: 'Documentation Parts'
};

export const Tips = () => <Tip>Don't stick your hand in a blender</Tip>;
export const CodeBlock = () => <Code>x = x * 2</Code>;
export const LinkInTip = () => <TipLink url="https://gis.utah.gov">Utah GIS</TipLink>;
export const NormalLink = () => <Link url="https://gis.utah.gov">Utah GIS</Link>;
export const Labels = () => <Label type="string">Label</Label>;
export const Descriptions = () => <Description>Description</Description>;
export const Headings = () => <Heading>Heading</Heading>;
export const SubHeadings = () => <SubHeading>SubHeading</SubHeading>;
