using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Services;

namespace api.mapserv.utah.gov.Cache
{
    public class LookupCache: ILookupCache
    {
        private readonly Dictionary<string, double[]> exclusions = new Dictionary<string, double[]>
            {
                {"bryce canyon", new[] {397995.510155659, 4170226.5544169028}},
                {"alta", new[] {446372D, 4493558D}},
                {"big water", new[] {440947.64679022622, 4103827.0225703362}},
                {"boulder", new[] {462787.32382167457, 4195803.554941956}}
            };

        public Dictionary<string, List<GridLinkable>> PlaceGrids { get; set; }

        public Dictionary<string, List<GridLinkable>> ZipCodesGrids { get; set; }

        public Dictionary<string, List<GridLinkable>> UspsDeliveryPoints { get; set; }

        public Dictionary<int, PoBoxAddress> PoBoxes { get; set; }

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
            //UspsDeliveryPoints = BuildGridLinkableLookup(deliveryPoints);
            //PoBoxes = poboxes;

            //var exclusions = corrections.ToList();
            //PoBoxZipCodesWithExclusions = exclusions.Select(x => x.Zip).Distinct();
            //PoBoxExclusions = exclusions.ToDictionary(x => x.ZipPlusFour, y => y);
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

        private IEnumerable<PoBoxAddressCorrection> GetCorrections(string spreadsheet, string range)
        {
            var items = new List<PoBoxAddressCorrection>();

            //foreach (var row in values)
            //{
            //    var city = row[5].ToString().ToLower();

            //    if (!exclusions.ContainsKey(city))
            //    {
            //        continue;
            //    }

            //    var location = exclusions[city];
            //    var zip5 = Convert.ToInt32(row[7]);
            //    var zip9 = Convert.ToInt32(row[1]);

            //    items.Add(new PoBoxAddressCorrection(zip5, zip9, location[0], location[1]));
            //}

            return items;
        }
    }
}
