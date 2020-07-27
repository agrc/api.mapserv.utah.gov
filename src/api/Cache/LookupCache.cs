using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AGRC.api.Models;
using AGRC.api.Models.Linkables;

namespace AGRC.api.Cache {
    public class LookupCache : ILookupCache {
        public LookupCache(ICacheRepository repo) {
            Repo = repo;
        }

        public ICacheRepository Repo { get; }
        public IDictionary<string, List<GridLinkable>> PlaceGrids { get; set; }
        public IDictionary<string, List<GridLinkable>> ZipCodesGrids { get; set; }
        public IDictionary<string, List<GridLinkable>> UspsDeliveryPoints { get; set; }
        public IDictionary<int, PoBoxAddress> PoBoxes { get; set; }
        public IReadOnlyCollection<int> PoBoxZipCodesWithExclusions { get; set; }
        public IDictionary<int, PoBoxAddressCorrection> PoBoxExclusions { get; set; }

        public async Task InitializeAsync() {
            PlaceGrids = BuildGridLinkableLookup(await Repo.GetPlaceNames());
            ZipCodesGrids = BuildGridLinkableLookup(await Repo.GetZipCodes());
            UspsDeliveryPoints = BuildGridLinkableLookup(await Repo.GetDeliveryPoints());
            PoBoxes = await Repo.GetPoBoxes();

            var exclusions = await Repo.GetCorrections();
            var exclusionsList = exclusions.ToList();
            PoBoxZipCodesWithExclusions = exclusionsList.Select(x => x.Zip).Distinct().ToArray();
            PoBoxExclusions = exclusionsList.ToDictionary(x => x.ZipPlusFour, y => y);
        }

        private static Dictionary<string, List<GridLinkable>> BuildGridLinkableLookup(
            IEnumerable<GridLinkable> gridLookup) {
            var dictionary = new Dictionary<string, List<GridLinkable>>();

            foreach (var item in gridLookup) {
                if (dictionary.ContainsKey(item.Key)) {
                    dictionary[item.Key].Add(item);
                    continue;
                }

                dictionary.Add(item.Key, new List<GridLinkable> {item});
            }

            return dictionary;
        }
    }
}
