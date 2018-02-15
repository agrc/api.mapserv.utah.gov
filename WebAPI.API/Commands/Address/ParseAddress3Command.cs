using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WebAPI.Common.Abstractions;
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
                NeedsSplit = App.RegularExpressions["separateNameAndDirection"].IsMatch(_value);

                // TODO: determine value type
                // eg: number, direction, street type, unit, po box, highway
                // maybe create an enum of types?

                IsNumber = new Regex("^[0-9]+$").IsMatch(value);
                IsDirection = App.RegularExpressions["direction"].IsMatch(_value) || App.RegularExpressions["directionSubstitutions"].IsMatch(_value);
                IsStreetType = App.RegularExpressions["streetType"].IsMatch(_value);
                IsHighway = App.RegularExpressions["highway"].IsMatch(_value);
                // TODO: should ordinals be considered numbers?
                IsOrdinal = App.RegularExpressions["ordinal"].IsMatch(_value);

                // TODO: based on type replace with correction or have other code do this?
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
                return _value;
            }
        }
    }
}