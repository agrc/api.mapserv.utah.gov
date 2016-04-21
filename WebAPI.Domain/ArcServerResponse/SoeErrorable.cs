namespace WebAPI.Domain.ArcServerResponse
{
    public abstract class SoeErrorable
    {
        public virtual SoeError Error { get; set; }

        public virtual bool IsSuccessful
        {
            get { return Error == null; }
        }

        public class SoeError
        {
            public int Code { get; set; }
            public string Message { get; set; }
        }
    }
}