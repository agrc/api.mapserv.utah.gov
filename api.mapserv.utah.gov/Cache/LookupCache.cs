using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Models;

namespace api.mapserv.utah.gov.Cache
{
    public class LookupCache : ILookupCache
    {
        public Dictionary<string, List<GridLinkable>> PlaceGrids { get; set; }

        public Dictionary<string, List<GridLinkable>> ZipCodesGrids { get; set; }

        public Dictionary<string, List<GridLinkable>> UspsDeliveryPoints { get; set; }

        public IDictionary<int, PoBoxAddress> PoBoxes { get; set; }

        public IEnumerable<int> PoBoxZipCodesWithExclusions { get; set; }

        public Dictionary<int, PoBoxAddressCorrection> PoBoxExclusions { get; set; }

        public ICacheRepository Repo { get; }

        public LookupCache (ICacheRepository repo){
            Repo = repo;
        }

        public async Task InitializeAsync()
        {
            PlaceGrids = BuildGridLinkableLookup(await Repo.GetPlaceNames());
            ZipCodesGrids = BuildGridLinkableLookup(await Repo.GetZipCodes());
            UspsDeliveryPoints = BuildGridLinkableLookup(await Repo.GetDeliveryPoints());
            PoBoxes = await Repo.GetPoBoxes();

            var exclusions = await Repo.GetCorrections();
            PoBoxZipCodesWithExclusions = exclusions.Select(x => x.Zip).Distinct();
            PoBoxExclusions = exclusions.ToDictionary(x => x.ZipPlusFour, y => y);
        }

        private static Dictionary<string, List<GridLinkable>> BuildGridLinkableLookup(
            IEnumerable<GridLinkable> gridLookup)
        {
            var dictionary = new Dictionary<string, List<GridLinkable>>();

            foreach (var item in gridLookup)
            {
                if (dictionary.ContainsKey(item.Key))
                {
                    dictionary[item.Key].Add(item);
                    continue;
                }

                dictionary.Add(item.Key, new List<GridLinkable> { item });
            }

            return dictionary;
        }
    }
}
