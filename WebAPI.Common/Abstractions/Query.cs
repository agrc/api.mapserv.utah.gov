using Raven.Client;

namespace WebAPI.Common.Abstractions
{
    public abstract class Query<T> : Command<T>
    {
        protected Query(IDocumentSession session)
        {
            Session = session;
        }

        public IDocumentSession Session { get; set; }
    }
}