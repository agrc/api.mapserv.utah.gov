using System.Text.RegularExpressions;
using WebAPI.Common.Abstractions;

namespace WebAPI.API.Commands.Geocode
{
    public class StandardizeRouteNameCommand : Command<string>
    {
        public StandardizeRouteNameCommand(string route)
        {
            Route = route;
        }

        public string Route { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, Route: {1}", "StandardizeRouteNameCommand", Route);
        }

        protected override void Execute()
        {
            var regex = new Regex(@"\d+");

            var matches = regex.Matches(Route);

            if (matches.Count == 0 || matches.Count > 1 || !matches[0].Success)
            {
                Result = null;
                return;
            }

            var route = matches[0].Value;

            Result = route.PadLeft(4, '0');
        }
    }
}