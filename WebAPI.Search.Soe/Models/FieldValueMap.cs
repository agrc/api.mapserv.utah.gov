namespace WebAPI.Search.Soe.Models
{
    public class FieldValueMap
    {
        public FieldValueMap(string field, object value)
        {
            Field = field;
            Value = value;
        }

        public string Field { get; set; }
        public object Value { get; set; }
    }
}