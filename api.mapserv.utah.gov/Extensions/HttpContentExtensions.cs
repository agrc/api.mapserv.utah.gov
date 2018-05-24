using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace api.mapserv.utah.gov.Extensions
{
  public static class HttpContentExtensions
  {
      public static async Task<T> ReadAsAsync<T>(this HttpContent httpContent)
      {
          var data = JsonConvert.DeserializeObject(await httpContent.ReadAsStringAsync().ConfigureAwait(false), typeof(T));

          return (T)data;
      }
  }
}
