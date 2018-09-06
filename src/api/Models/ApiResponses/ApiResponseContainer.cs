using Newtonsoft.Json;
using ProtoBuf;

namespace api.mapserv.utah.gov.Models.ApiResponses {
    [ProtoContract]
    public class ApiResponseContainer {
        [ProtoMember(1)]
        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }

        [ProtoMember(2)]
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        public bool ShouldSerializeMessage() => !string.IsNullOrEmpty(Message);
    }

    [ProtoContract]
    [ProtoInclude(3, typeof(ApiResponseContainer))]
    public class ApiResponseContainer<T> : ApiResponseContainer where T : class {
        [ProtoMember(4)]
        [JsonProperty(PropertyName = "result")]
        public T Result { get; set; }
    }
}
