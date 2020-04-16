import React from 'react';
import { Item } from './Elements';

export default function Goals() {
  return (
    <section className="mb-16 md:mb-0 grid grid-cols-2 md:grid-cols-3 lg:grid-cols-9 gap-6 mx-auto my-6 text-gray-700">
      <Item>Locally sourced data</Item>
      <Item>Address Reversals</Item>
      <Item>Delivery Points</Item>
      <Item>Mileposts</Item>
      <Item>P.O. Boxes</Item>
      <Item>Mapping library integration</Item>
      <Item>Open License</Item>
      <Item>Free</Item>
      <Item>No Restrictions</Item>
    </section>
  );
}
