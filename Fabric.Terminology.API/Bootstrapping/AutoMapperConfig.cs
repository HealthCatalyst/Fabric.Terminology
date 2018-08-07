namespace Fabric.Terminology.API.Bootstrapping
{
    using AutoMapper;

    using Fabric.Terminology.API.Bootstrapping.MapperProfiles;

    /// <summary>
    /// Utility class to ensure property AutoMapper configuration
    /// </summary>
    /// <seealso cref="https://github.com/AutoMapper/AutoMapper/issues/2607"/>
    public static class AutoMapperConfig
    {
        private static readonly object ConfigLock = new object();

        private static bool initialized = false;

        // Centralize automapper initialize
        public static void Initialize()
        {
            // This will ensure one thread can access to this static initialize call
            // and ensure the mapper is reseted before initialized
            lock (ConfigLock)
            {
                if (initialized)
                {
                    return;
                }

                Mapper.Initialize(
                    cfg =>
                        {
                            cfg.AddProfile<CodeSystemApiProfile>();
                            cfg.AddProfile<CodeSystemCodeApiProfile>();
                            cfg.AddProfile<ValueSetApiProfile>();
                        });

                initialized = true;
            }
        }
    }
}