using System;
using System.Threading;

namespace Fabric.Terminology.SqlServer.Threading
{
    internal class UpgradeableReaderLock : IDisposable
    {
        private readonly ReaderWriterLockSlim locker;
        private bool upgraded;

        public UpgradeableReaderLock(ReaderWriterLockSlim locker)
        {
            this.locker = locker;
            this.locker.EnterUpgradeableReadLock();
        }

        public void UpgradeToWriteLock()
        {
            this.locker.EnterWriteLock();
            this.upgraded = true;
        }

        void IDisposable.Dispose()
        {
            if (this.upgraded)
            {
                this.locker.ExitWriteLock();
            }

            this.locker.ExitUpgradeableReadLock();
        }
    }
}