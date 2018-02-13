using System;
using System.Linq;
using WebAPI.Common.Abstractions;
using WebAPI.Domain;
using WebAPI.Domain.Addresses;

namespace WebAPI.API.Commands.Address
{
    /// <summary>
    /// Pechmines style. Doesn't work that well.
    /// </summary>
    public class ParseAddressCommand2 : Command<AddressBase>
    {
        public ParseAddressCommand2()
        {
            Address =  new CleansedAddress();
        }

        public ParseAddressCommand2(string street)
        {
            SetStreet(street);
            Address = new CleansedAddress
                {
                    InputAddress = Street
                };
        }

        public string Street { get; set; }

        private AddressBase Address { get; set; }

        public string OriginalStreet { get; set; }

        public string StandardStreet { get; set; }

        public void SetStreet(string street)
        {
            Street = street;
            StandardStreet = Street;
            OriginalStreet = Street;
            Address.InputAddress = Street;
        }

        public override string ToString()
        {
            return
                $"ParseAddressCommand2, Street: {Street}, OriginalStreet: {OriginalStreet}, StandardStreet: {StandardStreet}";
        }

        protected override void Execute()
        {
            var state = new ParseState();

            foreach (var token in Street.Split(' ', '\t', ','))
            {
                state = ParseToken(token, state);
            }

            Result = Address;
        }

        private ParseState ParseToken(string token, ParseState state)
        {
            switch (state)
            {
                case ParseState.HouseNumberState:
                    try
                    {
                        Address.HouseNumber = Convert.ToInt32(token);
                    }
                    catch (Exception e)
                    {
                        throw new FormatException("Error parsing presumed house number '" + token + "'", e);
                    }

                    state = ParseState.PrefixDirectionState;
                    break;
                case ParseState.PrefixDirectionState:
                    Address.PrefixDirection = ParseDirection(token);
                    state = ParseState.StreetNameNeeded;
                    if (Direction.None != Address.PrefixDirection)
                        break;
                    
                    goto case ParseState.StreetNameNeeded; // prefix directions may be omitted/skipped
                case ParseState.StreetNameNeeded:
                    Address.StreetName = token;
                    state = ParseState.StreetNameBegun;
                    break;
                case ParseState.StreetNameBegun:
                    // handle multiword street names by concatenating them until a suffix direction
                    // or street type is encountered
                    Direction dir;
                    if (Direction.None != (dir = ParseDirection(token)))
                        Address.SuffixDirection = dir;
                    else
                    {
                        StreetType stType;
                        if (StreetType.None != (stType = ParseStreetType(token)))
                            Address.StreetType = stType;
                        else
                        {
                            Address.StreetName = Address.StreetName + " " + token;
                            break;
                        }
                    }

                    state = ParseState.Complete;
                    break;
            }
            return state;
        }

        private static StreetType ParseStreetType(string token)
        {
            if (App.StreetTypeAbbreviations == null || App.StreetTypeAbbreviations.Count < 1)
            {
                CacheConfig.BuildCache();
            }

            var normalizedToken = token.ToLower();

            if (App.StreetTypeAbbreviations == null || !App.StreetTypeAbbreviations.Any(x => x.Value.Contains(normalizedToken)))
            {
                return StreetType.None;
            }

            return App.StreetTypeAbbreviations.SingleOrDefault(x => x.Value.Contains(normalizedToken)).Key;
        }

        private static Direction ParseDirection(string token)
        {
            var normalizedToken = token.ToLower();

            if ("north" == normalizedToken || "n" == normalizedToken)
                return Direction.North;
            if ("east" == normalizedToken || "e" == normalizedToken)
                return Direction.East;
            if ("south" == normalizedToken || "s" == normalizedToken)
                return Direction.South;
            if ("west" == normalizedToken || "w" == normalizedToken)
                return Direction.West;
            return Direction.None;
        }

        private enum ParseState
        {
            HouseNumberState,
            PrefixDirectionState,
            StreetNameNeeded,
            StreetNameBegun,
            Complete
        }
    }
}