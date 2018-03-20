namespace Fabric.Terminology.Domain
{
    using System;

    using CallMeMaybe;

    using JetBrains.Annotations;

    public class Attempt<T>
    {
        public bool Success { get; private set; }

        [CanBeNull]
        public T Result { get; private set; }

        [CanBeNull]
        public Exception Exception { get; private set; }

        public static Attempt<T> Successful(T result)
        {
            return new Attempt<T> { Success = true, Result = result };
        }

        public static Attempt<T> Failed(Exception ex)
        {
            return Attempt<T>.Failed(ex, default(T));
        }

        public static Attempt<T> Failed(Exception ex, [CanBeNull] T result)
        {
            return new Attempt<T>
            {
                Success = false,
                Result = result,
                Exception = ex
            };
        }
    }
}