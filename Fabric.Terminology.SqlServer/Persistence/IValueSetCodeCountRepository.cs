namespace Fabric.Terminology.SqlServer.Persistence
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using Fabric.Terminology.Domain.Models;

	public interface IValueSetCodeCountRepository
    {
        IReadOnlyCollection<IValueSetCodeCount> GetValueSetCodeCounts(Guid valueSetGuid);

        Task<Dictionary<Guid, IReadOnlyCollection<IValueSetCodeCount>>> BuildValueSetCountsDictionary(IEnumerable<Guid> valueSetGuids);
    }
}