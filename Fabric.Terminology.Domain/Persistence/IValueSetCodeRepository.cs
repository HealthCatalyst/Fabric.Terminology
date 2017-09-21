namespace Fabric.Terminology.Domain.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Fabric.Terminology.Domain.Models;

    public interface IValueSetCodeRepository
    {
        int CountValueSetCodes(Guid valueSetGuid, IEnumerable<Guid> codeSystemGuids);

        IReadOnlyCollection<IValueSetCode> GetValueSetCodes(Guid valueSetGuid);

        IReadOnlyCollection<IValueSetCodeCount> GetValueSetCodeCounts(Guid valueSetGuid);

        IReadOnlyCollection<IValueSetCode> GetValueSetCodes(Guid valueSetGuid, IEnumerable<Guid> codeSystsemGuids);

        Task<Dictionary<Guid, IReadOnlyCollection<IValueSetCodeCount>>> BuildValueSetCountsDictionary(IEnumerable<Guid> valueSetGuids);

        Task<Dictionary<Guid, IReadOnlyCollection<IValueSetCode>>> BuildValueSetCodesDictionary(IEnumerable<Guid> valueSetGuids);
    }
}