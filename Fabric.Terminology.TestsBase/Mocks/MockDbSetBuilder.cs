namespace Fabric.Terminology.TestsBase.Mocks
{
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.TestsBase.Fake;

    using Microsoft.EntityFrameworkCore;

    using Moq;

    public class MockDbSetBuilder<TEntity>
        where TEntity : class
    {
        public Mock<DbSet<TEntity>> Build(IEnumerable<TEntity> data)
        {
            var enumerable = data as TEntity[] ?? data.ToArray();
            var queryable = enumerable.AsQueryable();

            var mockDbSet = new Mock<DbSet<TEntity>>();
            mockDbSet.As<IAsyncEnumerable<TEntity>>()
                .Setup(d => d.GetEnumerator())
                .Returns(new AsyncEnumerator<TEntity>(queryable.GetEnumerator()));

            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            return mockDbSet;
        }
    }
}