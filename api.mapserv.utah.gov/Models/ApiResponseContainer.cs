using Newtonsoft.Json;

namespace api.mapserv.utah.gov.Models
{
    public class ApiResponseContainer
    {
        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        public bool ShouldSerializeMessage() => !string.IsNullOrEmpty(Message);
    }

    public class ApiResponseContainer<T> : ApiResponseContainer where T : class
    {
        [JsonProperty(PropertyName = "result")]
        public T Result { get; set; }
    }
}