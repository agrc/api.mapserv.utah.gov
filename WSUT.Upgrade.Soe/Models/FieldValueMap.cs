using Newtonsoft.Json;

namespace WSUT.Upgrade.Soe.Models
{
  public class FieldValueMap
    {
        private object _value;

        public FieldValueMap(string field, object value)
        {
            Field = field;
            Value = value;
        }

        [JsonProperty("field")]
        public string Field { get; set; }

        [JsonProperty("value")]
        public object Value
        {
            get
            {
                return _value ?? "";
            }
            set { _value = value; }
        }
    }
}