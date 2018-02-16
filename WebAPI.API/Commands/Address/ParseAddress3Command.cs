using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WebAPI.API.Comparers;
using WebAPI.Common.Abstractions;
using WebAPI.Domain;
using WebAPI.Domain.Addresses;

namespace WebAPI.API.Commands.Address
{
    public class ParseAddress3Command : Command<CleansedAddress>
    {
        private readonly string _original;
        private readonly string _street;

        public ParseAddress3Command(string street)
        {
            _street = _original = street;
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        protected override void Execute()
        {
            var street = RemoveExtraniousCharacters(_street);
            var parts = CreateStreetParts(street);
        }

        public string RemoveExtraniousCharacters(string street)
        {
            // TODO: keep # for unit numbers?
            // TODO: do street names contain hyphens?
            var alphaNumeric = new Regex("[^0-9|a-z|\\s]");

            return alphaNumeric.Replace(_street, "");
        }

        public IEnumerable<StreetPart> CreateStreetParts(string street)
        {
            // TODO: create linked list so there is a first prop to get the starting point

            var parts = street.Split(' ');
            var streetParts = parts.Select((x, index) => new StreetPart(x, index));

            // TODO: split and insert new streeparts
            // test that the linked list is preserved with order
//            while (streetParts.Any(x => x.NeedsSplit))
//            {
//                // split and insert;
//            }

            var partHash = streetParts.ToDictionary(x => x.Index, y => y);
            var length = partHash.Keys.Count - 1;

            foreach (var pair in partHash)
            {
                var index = pair.Key;
                var part = pair.Value;

                if (index == 0 && index + 1 <= length)
                {
                    part.Link(null, partHash[index + 1]);
                    continue;
                }

                if (index == length)
                {
                    part.Link(partHash[index - 1], null);
                    continue;
                }

                part.Link(partHash[index - 1], partHash[index + 1]);
            }

            return partHash.Values.ToList();
        }

        public class StreetPart
        {
            private readonly string _value;

            public bool NeedsSplit { get; }

            public StreetPart(string value, int index)
            {
                Index = index;
                _value = value;

                // TODO: does contain multiple parts
                // eg: 123west 456south should be split from 2 to 4 parts
                // TODO: check failed addresses with units and such to see if this
                // regex is sufficient
                NeedsSplit = new Regex(@"(?<=\d)(?=\p{L})|(?<=\p{L})(?=\d)", RegexOptions.IgnoreCase).IsMatch(_value);

                // TODO: determine value type
                // eg: number, direction, street type, unit, po box, highway
                // maybe create an enum of types?

                IsNumber = new Regex("^[0-9]+$").IsMatch(value);
                IsDirection = App.RegularExpressions["direction"].IsMatch(_value) || App.RegularExpressions["directionSubstitutions"].IsMatch(_value);
                IsStreetType = App.RegularExpressions["streetType"].IsMatch(_value); 
                IsHighway = App.RegularExpressions["highway"].IsMatch(_value);
                if (IsHighway)
                {
                    _value = "Highway";
                }
                // TODO: should ordinals be considered numbers?
                IsOrdinal = App.RegularExpressions["ordinal"].IsMatch(_value);

                // TODO: based on type replace with correction or have other code do this?
            }

            public static StreetType GetStreetType(string value)
            {
                var abbr = value.ToLower();

                if (Enum.TryParse(abbr, true, out StreetType streetType))
                {
                    return streetType;
                }

                if (App.StreetTypeAbbreviations != null &&
                    App.StreetTypeAbbreviations.Values.Any(x => x.Split(',').Contains(abbr)))
                {
                    return App.StreetTypeAbbreviations
                        .Where(x => x.Value.Split(',').Contains(abbr, new StreetTypeAbbreviationComparer()))
                        .Select(x => x.Key)
                        .SingleOrDefault();
                }

                return streetType;
            }

            // TODO: need to be careful of single letter street names
            // eg: E,N,S,W streets that are also directions
            private Direction GetDirection(string value)
            {
                if (Enum.TryParse(_value, true, out Direction direction))
                {
                    return direction;
                }

                return direction;
            }

            public bool IsNumber { get; }

            public bool IsDirection { get; }

            public bool IsStreetType { get; }

            public bool IsHighway { get; }

            public bool IsOrdinal { get; }

            public int Index { get; }

            public StreetPart Left { get; private set; }

            public StreetPart Right { get; private set; }

            public bool IsFirst => Left == null;

            public bool IsLast => Right == null;

            public void Link(StreetPart left, StreetPart right)
            {
                Left = left;
                Right = right;
            }

            public object GetValue()
            {
                if (IsNumber)
                {
                    return Convert.ToInt32(_value);
                }

                if (IsStreetType)
                {
                    return GetStreetType(_value);
                }

                if (IsDirection)
                {
                    return GetDirection(_value);
                }

                return _value;
            }
        }
    }
}