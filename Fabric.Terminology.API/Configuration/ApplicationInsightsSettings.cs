namespace Fabric.Terminology.API.Configuration
{
    using NullGuard;

    public class ApplicationInsightsSettings
    {
        [AllowNull]
        public string InstrumentationKey { get; set; }

        public bool Enabled { get; set; }
    }
}