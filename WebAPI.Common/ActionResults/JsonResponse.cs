using System.Net;
using Newtonsoft.Json;

namespace WebAPI.Common.ActionResults
{
    public class JsonResponse<T>
    {
        public JsonResponse(T data, HttpStatusCode status)
        {
            Data = data;
            Status = status;
        }

        [JsonProperty(PropertyName = "data")]
        public T Data { get; set; }

        [JsonProperty(PropertyName = "status")]
        public HttpStatusCode Status { get; set; }
    }
}