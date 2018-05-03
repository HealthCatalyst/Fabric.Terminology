namespace Fabric.Terminology.TestsBase.Fake
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

#pragma warning disable CA1063 // Implement IDisposable Correctly
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
#pragma warning disable UseAsyncSuffix // Use Async suffix - has to implement the interface

    // https://stackoverflow.com/questions/39719258/idbasyncqueryprovider-in-entityframeworkcore
    public class AsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> enumerator;

        public AsyncEnumerator(IEnumerator<T> enumerator) => this.enumerator = enumerator;

        public T Current => this.enumerator.Current;

        public void Dispose()
        {
        }

        public Task<bool> MoveNext(CancellationToken cancellationToken) => Task.FromResult(this.enumerator.MoveNext());
    }
#pragma warning restore UseAsyncSuffix // Use Async suffix
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
#pragma warning restore CA1063 // Implement IDisposable Correctly
}