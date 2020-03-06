import React from 'react';
import EndpointInput from '../../EndpointInput';
import EndpointSelect from '../../EndpointSelect';
import EndpointSwitch from '../../EndpointSwitch';
import EndpointFormat from '../../EndpointFormat';
import EndpointAdvancedToggle from '../../EndpointAdvancedToggle';

export default function StreetZone() {
  return (
    <>
      <EndpointInput name="street" placeholder="123 main street" type="string" required="true" />
      <EndpointInput name="zone" placeholder="SLC" type="string" required={true} />
      <EndpointAdvancedToggle>
        <EndpointInput name="spatialReference" placeholder="26912" type="int" required={false} />
        <EndpointInput name="acceptScore" placeholder="70" type="int" required={false} />
        <EndpointSwitch name="pobox" placeholder="true" type="boolean" required={false} />
        <EndpointSelect name="locators" placeholder="all" type="string" required={false}>
          <option selected>all</option>
          <option>addressPoints</option>
          <option>roadCenterlines</option>
        </EndpointSelect>
        <EndpointInput name="suggest" placeholder="0" type="int" required={false} />
        <EndpointSwitch name="scoreDifference" placeholder="false" type="boolean" required={false} />
        <EndpointFormat />
        <EndpointInput name="callback" type="string" required={false} />
      </EndpointAdvancedToggle>
      </>
  );
}
