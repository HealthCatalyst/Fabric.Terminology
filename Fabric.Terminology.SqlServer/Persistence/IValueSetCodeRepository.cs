namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Fabric.Terminology.Domain.Models;

    public interface IValueSetCodeRepository
    {
        IReadOnlyCollection<IValueSetCode> GetValueSetCodes(Guid valueSetGuid);

        Task<Dictionary<Guid, IReadOnlyCollection<IValueSetCode>>> BuildValueSetCodesDictionary(
            IEnumerable<Guid> valueSetGuids);
    }
}