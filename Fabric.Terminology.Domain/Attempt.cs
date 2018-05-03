namespace Fabric.Terminology.Domain
{
    using System;

    using JetBrains.Annotations;

    using NullGuard;

#pragma warning disable CA1000 // Do not declare static members on generic types
    public class Attempt<T>
    {
        public bool Success { get; private set; }

        [AllowNull, CanBeNull]
        public T Result { get; private set; }

        [AllowNull, CanBeNull]
        public Exception Exception { get; private set; }

        public static Attempt<T> Successful(T result)
        {
            return new Attempt<T> { Success = true, Result = result };
        }

        public static Attempt<T> Failed(Exception ex)
        {
            return Attempt<T>.Failed(ex, default(T));
        }

        public static Attempt<T> Failed(Exception ex, [AllowNull, CanBeNull] T result)
        {
            return new Attempt<T>
            {
                Success = false,
                Result = result,
                Exception = ex
            };
        }
    }
#pragma warning restore CA1000 // Do not declare static members on generic types
}