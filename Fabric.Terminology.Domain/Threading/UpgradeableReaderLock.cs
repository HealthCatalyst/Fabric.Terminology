using System;
using System.Threading;

namespace Fabric.Terminology.Domain.Threading
{
    internal class UpgradeableReaderLock : IDisposable
    {
        private readonly ReaderWriterLockSlim _locker;
        private bool _upgraded;

        public UpgradeableReaderLock(ReaderWriterLockSlim locker)
        {
            _locker = locker;
            _locker.EnterUpgradeableReadLock();
        }

        public void UpgradeToWriteLock()
        {
            _locker.EnterWriteLock();
            _upgraded = true;
        }

        void IDisposable.Dispose()
        {
            if (_upgraded)
            {
                _locker.ExitWriteLock();
            }

            _locker.ExitUpgradeableReadLock();
        }
    }
}