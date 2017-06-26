namespace Fabric.Terminology.SqlServer.Configuration
{
    public interface IMemoryCacheSettings
    {
        bool MemoryCacheEnabled { get; set; }
        int MemoryCacheMinDuration { get; set; }
        bool MemoryCacheSliding { get; set; }
    }
}