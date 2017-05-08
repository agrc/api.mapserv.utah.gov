using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using WebAPI.Common.Models.Raven.Keys;

namespace WebAPI.Common.Indexes
{
    public class IndexLastUsedDate : AbstractIndexCreationTask<GeocodeStreetZoneUsage>
        {
            public IndexLastUsedDate()
            {
                Map = requests => from request in requests
                              select new
                              {
                                  request.LastUsedTicks
                              };

                Index(x => x.LastUsedTicks, FieldIndexing.Analyzed);
            }
        } 
    }