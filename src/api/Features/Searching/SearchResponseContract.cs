using System.Collections.Generic;
using System.Text.Json;

namespace AGRC.api.Features.Searching {
    public class SearchResponseContract {
        public JsonElement Geometry { get; set; }

        public IDictionary<string, object> Attributes { get; set; }
    }
}
