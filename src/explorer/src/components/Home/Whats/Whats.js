import React from 'react';
import { H1 } from './Elements';
import { P } from '../Elements';

export default function Whats() {
  return (
    <div className="grid grid-cols-1 md:grid-cols-2 gap-5 text-left font-light text-gray-600 leading-relaxed tracking-wide">
      <div>
        <H1>What is geocoding</H1>
        <P className="ml-3">
          Geocoding allows you to find a geographic location (i.e., the geographical coordinates) from an address. AGRC’s geocoding philosophy is unique in that
          we would rather give you no match than the wrong match. But we assure you: we will have the best match rates for Utah addresses.
        </P>
        <P className="ml-3 mt-5">How can we be so sure?—Because we’re the experts in Utah geospatial data.</P>
      </div>
      <div className="hidden md:flex justify-center self-center">
        <svg className="float-left mr-2" xmlns="http://www.w3.org/2000/svg" width="250" height="215.7" viewBox="0 0 1142 960">
          <defs />
          <defs>
            <linearGradient id="a" x1="535.5" x2="535.5" y1="779.2" y2="104.9" gradientUnits="userSpaceOnUse">
              <stop offset="0" stopColor="gray" stopOpacity=".3" />
              <stop offset=".5" stopColor="gray" stopOpacity=".1" />
              <stop offset="1" stopColor="gray" stopOpacity=".1" />
            </linearGradient>
            <linearGradient id="c" x1="873.2" x2="873.2" y1="742.8" y2="620.1" gradientUnits="userSpaceOnUse">
              <stop offset="0" stopOpacity=".1" />
              <stop offset=".6" stopOpacity=".1" />
              <stop offset="1" stopOpacity="0" />
            </linearGradient>
            <clipPath id="b" transform="translate(-65 -8)">
              <path fill="#ffba00" d="M889 735H725l-251 24-253-24V140l252 18 257-18 252 34v468l-93 93z" />
            </clipPath>
          </defs>
          <path fill="url(#a)" d="M829 753H669l-272 26-274-26V105l273 20 279-20 273 37-7 498-112 113z" />
          <path fill="#fff" d="M824 744H668l-267 26-268-26V113l267 20 273-20 267 37-7 484-109 110z" />
          <path fill="#ffba00" d="M825 727H660l-251 24-253-24V132l252 18 257-18 252 34v468l-92 93z" />
          <g clipPath="url(#b)">
            <path fill="#fff" d="M1142 905L564 585 711 97l-50-19-71 236L23 0 0 55l572 317-130 434 50 20 55-183 572 317 23-55z" />
          </g>
          <path fill="url(#c)" d="M813 743l2-123 118 5-120 118z" />
          <path fill="#fff" d="M156 389l363 161 24 50-24 25-363-173v-63zM824 743l2-113 107 4-109 109zM649 648l151-498 61 9-170 515-42-26z" />
          <path d="M408 751l252-24V132l-252 18" opacity=".1" />
          <path fill="#ff5252" d="M639 348c0 48-87 184-87 184s-87-136-87-184a87 87 0 11174 0z" />
          <circle cx="551.8" cy="342.4" r="26.3" fill="#fff" />
          <path fill="#fff" d="M785 389l132 34v64l-162-58 30-40zM279 511l-33 232 38 8 50-214v-30l-55 4zM278 209l-37 217 29 16 69-233h-61z" />
          <circle cx="350" cy="165.6" r="8" fill="#ff5252" />
          <circle cx="254" cy="579.6" r="8" fill="#ff5252" />
          <circle cx="246" cy="638.6" r="8" fill="#ff5252" />
          <circle cx="331" cy="624.6" r="8" fill="#ff5252" />
          <circle cx="194" cy="384.6" r="8" fill="#ff5252" />
          <circle cx="824" cy="376.6" r="8" fill="#ff5252" />
          <circle cx="755" cy="654.6" r="8" fill="#ff5252" />
        </svg>
      </div>
      <div className="hidden md:flex justify-center self-center">
        <svg className="float-left mr-2" xmlns="http://www.w3.org/2000/svg" width="250" height="215.7" viewBox="0 0 1105 798">
          <defs />
          <path fill="#3f3d56" d="M959 389h10v388h-10z" />
          <path fill="#3f3d56" d="M1060 438c0 136-95 246-95 246s-97-109-97-245 94-246 94-246 97 109 98 245zM767 571h5v200h-5z" />
          <path fill="#3f3d56" d="M819 597c0 69-49 126-49 126s-50-56-50-126 49-127 49-127 50 57 50 127z" />
          <ellipse cx="872" cy="776.5" fill="#3f3d56" rx="137" ry="21" />
          <path fill="#2f2e41" d="M883 287c-11-20-34-20-34-20s-23-3-37 27c-13 27-32 54-3 61l5-16 3 17a113 113 0 0013 0c31-1 60 1 59-11-1-14 5-39-6-58z" />
          <path fill="#fbbebe" d="M841 324s15 21 6 38 21 35 21 35l22-48s-26-17-19-33z" />
          <circle cx="851.5" cy="307" r="26" fill="#fbbebe" />
          <path fill="#fbbebe" d="M761 337l22-28s8-29 19-28-4 35-4 35l-22 31zM871 710l7 37 15 4-4-44-18 3zM1022 670l27 37 11 5 9-15-27-35-20 8z" />
          <path fill="#575a89" d="M865 381l-16-22s-33 6-36 9 8 58 8 58 2 15 11 23l9 6 74-15 3-33a86 86 0 00-23-66l-12 2z" />
          <path fill="#575a89" d="M817 370l-5-2-23-2s-8-2-6-6 4-5 0-6-5-2-4-5 7-9 7-9l-17-14-2 2c-8 7-35 31-16 56 23 29 50 46 72 40z" />
          <path
            fill="#2f2e41"
            d="M839 450v14s-9 17-6 33l4 24a137 137 0 007 40c7 20-16 151 13 153s45 4 54-6-15-182-15-182 82 171 99 164 60-23 55-32L919 449l-4-9zM887 740s-16-1-16 4-8 22-8 22-6 20 10 18 26-20 26-20l-4-19zM1054 705s-14-9-13-3 2 22 9 23 28 7 29 9 25 10 25-3-15-23-15-23l-17-13s-11-1-13 6-5 4-5 4z"
          />
          <circle cx="851.5" cy="266.7" r="16.6" fill="#2f2e41" />
          <path fill="#2f2e41" d="M833 262a17 17 0 0115-16 17 17 0 00-2 0 17 17 0 100 33 17 17 0 002 0 17 17 0 01-15-17z" />
          <path fill="#2f2e41" d="M878 285l-23-12-31 5-6 29 16-1 4-10v10h8l4-17 3 18 26-1-1-21z" />
          <path fill="#fbbebe" d="M877 452l-35 1s-30 6-28-6 30-8 30-8l32-5z" />
          <path fill="#575a89" d="M904 339a7 7 0 019 5c7 22 28 94 7 104-25 12-43 11-43 11l-10-23 9-9 9-60 5-25z" />
          <path d="M898 380l-4 40-35 15 39-9v-46z" opacity=".4" />
          <path fill="#3f3d56" d="M709 247a47 47 0 0128 20l3 5v1l57 16a4 4 0 013 6l-4 16a4 4 0 01-5 3l-58-16-6 2a52 52 0 01-33 1z" />
          <ellipse cx="811.1" cy="344.2" fill="#575a89" rx="3.5" ry="6.6" transform="rotate(-74 753 350)" />
          <circle cx="766.7" cy="294.1" r="2.4" fill="#3f3d56" />
          <ellipse cx="749.7" cy="325.7" fill="#ffba00" rx="28.1" ry="12.7" transform="rotate(-72 691 333)" />
          <path d="M712 278c5-14 4-28-2-30a6 6 0 011 0c6 2 8 16 3 30s-14 25-21 23c6 1 15-9 19-23z" opacity=".4" />
          <path
            fill="#ffba00"
            d="M710 248L580 146a862 862 0 01-66-57c-38-35-88-64-147-79A261 261 0 0060 134C-18 280-78 516 270 390c76-27 153-30 211-45l212-43z"
          />
          <circle cx="319" cy="70.5" r="7" fill="#f2f2f2" />
          <path
            fill="#f2f2f2"
            d="M455 87h-2v-2 2h-2 2v2-2h2zM638 239h-2v-2 2h-2 2v2-2h2zM625 286h-2v-2 2h-2 2v2-2h2zM66 326h-2v-2 2h-2 2v2-2h2zM146 108h-2v-2 2h-2 2v2-2h2zM457 294h-2v-2 2h-2 2v2-2h2zM266 44h-2v-2 2h-2 2v2-2h2z"
          />
          <path
            fill="#e6e6e6"
            d="M267 188h-3l2 14h2l-1-14zM260 160l-2 1 4 13h2l-4-14zM251 134l-3 1 6 13 2-1-5-13zM237 110l-2 1 7 12 2-1c-2-4-4-9-7-12zM220 87l-1 1 8 11 2-1-9-11zM201 67l-2 2 10 9 2-1-10-10zM179 50l-2 2 12 8 1-2-11-8zM159 38v2l7 4 1-2-8-4zM267 216h2v7h-2v-7zM132 385h-3l2 14h2l-1-14zM125 357l-2 1 4 13h2l-4-14zM116 331l-3 1 6 13 2-1-5-13zM102 307l-2 1 7 12 2-1c-2-4-4-9-7-12zM85 284l-1 1 8 11 2-1-9-11zM66 264l-2 2 10 9 2-1-10-10zM44 247l-2 2 12 8 1-2-11-8zM24 235v2l7 4 1-2-8-4zM132 413h2v7h-2v-7zM594 286h-3l2 14h2l-1-14zM587 258l-2 1 4 13h2l-4-14zM578 232l-3 1 6 13 2-1-5-13zM564 208l-2 1 7 12 2-1c-2-4-4-9-7-12zM547 185l-1 1 8 11 2-1-9-11zM528 165l-2 2 10 9 2-1-10-10zM506 148l-2 2 12 8 1-2-11-8zM486 136v2l7 4 1-2-8-4zM594 314h2v7h-2v-7z"
          />
          <path fill="#f2f2f2" d="M149 200h-1v-2h-1v2h-1 1v2h1v-2h1zM130 302h-2v-1 1h-2v1h2v1-1h2v-1zM72 392h-1v-2h-1v2h-1 1v2h1v-2h1z" />
          <path fill="#3f3d56" d="M232 278a123 123 0 1157 46 40 40 0 11-57-46z" />
          <path
            fill="#ffba00"
            d="M333 139c28 0 48 16 48 39 0 14-8 25-21 33-14 7-18 13-18 23a6 6 0 01-6 6h-14a6 6 0 01-6-6v-1c-1-15 4-25 18-33 12-8 18-12 18-22s-9-16-21-16c-8 0-15 4-18 11a13 13 0 01-12 8c-10 0-16-11-12-19 7-14 23-23 44-23zm-20 129c0-9 8-15 16-15 9 0 17 6 17 15s-8 16-17 16-16-7-16-16z"
          />
        </svg>
      </div>
      <div>
        <H1>What is searching</H1>
        <P className="ml-3">
          Searching allows you to search through over a 1,000,000 rows of SGID data. With over 300 layers of real-world data that you can run queries against in
          the SGID, the possibilities can be a little overwhelming. But the searching endpoint really opens up the opportunity for serious information
          gathering. You need a way to sift through all that information and make sense of it, and our search endpoint can help you do that.
        </P>
      </div>
    </div>
  );
}
