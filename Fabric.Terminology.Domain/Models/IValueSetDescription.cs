namespace Fabric.Terminology.Domain.Models
{
    public interface IValueSetDescription : IValueSetItem
    {
        // TODO Discuss what these values should be for custom ValueSets 

        string AuthoringSourceDescription { get; }
        string PurposeDescription { get; }
        string SourceDescription { get; }
        string VersionDescription { get; }


        // other fields to consider
        // string Status { get; }
        // bool Public { get; }
        // DateTime LastLoadDate { get; }
    }
}