namespace Fabric.Terminology.API.Infrastructure
{
    using Nancy;

    public class NancyContextWrapper
    {
        public NancyContextWrapper(NancyContext context)
        {
            this.Context = context;
        }

        public NancyContext Context { get; internal set; }
    }
}