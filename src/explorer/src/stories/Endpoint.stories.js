import React from 'react';
import Endpoint from '../components/Endpoint/Endpoint';
import EndpointDemoDocToggle from '../components/Endpoint/EndpointDemoDocToggle';
import EndpointResponseFormat from '../components/Endpoint/EndpointResponseFormat';
import EndpointAdvancedToggle from '../components/Endpoint/EndpointAdvancedToggle';
import EndpointInput from '../components/Endpoint/EndpointInput';
import { TypeLabel, RequiredLabel } from '../components/Endpoint/EndpointLabel';
import EndpointSelect from '../components/Endpoint/EndpointSelect';
import EndpointSwitch from '../components/Endpoint/EndpointSwitch';
import EndpointUrl from '../components/Endpoint/EndpointUrl';
import EndpointResponse from '../components/Endpoint/EndpointResponse';

export default {
  title: 'Endpoint Parts',
  component: Endpoint
};

const sampleResponse = {
  result: {
    location: {
      x: 424818.8714090543,
      y: 4513223.695653789
    },
    score: 90.92,
    locator: 'AddressPoints.AddressGrid',
    matchAddress: '123 S MAIN ST, SALT LAKE CITY',
    inputAddress: '123 south main, slc',
    addressGrid: 'SALT LAKE CITY'
  },
  status: 200
};

export const DemoDocToggle = () => <EndpointDemoDocToggle active="docs" showApi={() => {}}></EndpointDemoDocToggle>;
export const ResponseFormat = () => <EndpointResponseFormat></EndpointResponseFormat>;
export const AdvancedToggle = () => <EndpointAdvancedToggle></EndpointAdvancedToggle>;
export const Input = () => <EndpointInput type="int" placeholder="placeholder" required={true}></EndpointInput>;
export const Label = () => (
  <div className="w-1/6">
    <div className="flex md:flex-col pt-2 md:pt-0 align-center opacity-75">
      <TypeLabel type="string"></TypeLabel>
      <RequiredLabel required={false}></RequiredLabel>
    </div>
    <div className="flex md:flex-col pt-2 md:pt-0 align-center opacity-75 md:mt-2">
      <TypeLabel type="string"></TypeLabel>
      <RequiredLabel required={true}></RequiredLabel>
    </div>
  </div>
);
export const Select = () => (
  <EndpointSelect type="int" required={false}>
    <option>options</option>
  </EndpointSelect>
);
export const Switch = () => <EndpointSwitch type="boolean" required={true}></EndpointSwitch>;
export const Code = () => <EndpointResponse code={JSON.stringify(sampleResponse, null, 2)}></EndpointResponse>;
export const Url = () => (
  <EndpointUrl url="https://longurlmaker.com/go?id=f0cShorlEasyURLdrawnZoutx0e010sspunZoutMyURL3high96ShrinkURL0f1longishadrawnZout11lofty298drawnZout49p9011aStartURL1toweringfy1GetShorty8continued0o33eB652005outstretchedefarZreachingclankyprotracted5007ncYATUC9deepprolonged7b3f4fc036ShredURL31bstretch89k0expanded56SmallrCanURL392LiteURL00crunning8073runningr5185rShim0bDoiopYATUCfMetamarkGetShorty1running1e9v9240sustainedp0SitelutionsrangydrawnZout520ShortlinksIsZgd622c002elongated80continuedblengthy5URLZcoZukgreatrangy8ShimUrlTea6running11runnings364farZoff8TightURL9e587elongatedaB6519929f4WapURLShortURL42farZoff1SmallrextensiveXilprolonged5d2deep7Ne1tall9a60protractedrangy5G8LShredURLif13c1ShrinkURL78enduring55004102lingeringb1Smallru1elongateXZsehprolonged8zcontinuedprolonged314GetShorty910150GetShortyShim7lanky2SnipURL1longishtalllspunZoutFhURL9SnipURL7sShrinkr6MyURLdrawnZoutURlZie3Sitelutionsprotracted5dcfarZreaching112Shorl03bspunZout0d5astretchingShrinkrloftyMooURL2001farawayfarZreaching1enlarged02cg1rangy6URLPiea9r600farZoffb18079fStartURL7stretchinglengthyShortlinksprotracted122running2qextensiveesustained811110ShoterLink1DigBig4Sitelutions13785yenlarged0enlarged6003lnkZinqy732251516Shim6farZoff069tall139MinilienoeShortURLa7DigBig09eDecentURLPiURLSmallr729NutshellURL0Ulimiti2greatfarawaySnipURLj0h700TraceURL01m330DigBig951FwdURL11SnipURLIsZgdc6TraceURLSmallrUrlTeasustainedhB65EzURL70s244deep14sustained1StartURLB65fffarawayganglingqcontinuedprolongedfarZoff1a8016DoiopFhURLf48towering2c8deepstretchedf10cremoteefarawayMyURLeXZse016b1w71d0l2688farZreaching185kfeURLcutNotLongexpandedSHurlrUlimitfarZofff8distantEzURL3stretched515stringynelongatedqdistant1ctallf88lastingb1LiteURLA2N00bloutstretched1continuedLiteURLfarZreaching1BeamZtod6hDigBiggreat0vganglingNotLongenduring600UrlTea1RubyURLlasting1aloftysustainedp07iTinyURL0enlargedGetShortydrawnZout1SHurla0f04ce1BeamZtoc12farZoff7stretchedexpandedMinilienu17x40d4YepIt14FwdURLfShortURL5511towering73stringy0elongate82d691lanky1elongate8Ne101EzURLelongatefeURLPie2extensivelingeringe31chURLPieTightURLNotLong1URLviec"></EndpointUrl>
);
