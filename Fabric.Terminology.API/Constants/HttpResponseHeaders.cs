namespace Fabric.Terminology.API.Constants
{
    using System;
    using System.Collections.Generic;

    public static class HttpResponseHeaders
    {
        public static readonly string Location = "Location";

        public static IEnumerable<Tuple<string, string>> CorsHeaders { get; } = new[]
        {
            new Tuple<string, string>("Access-Control-Allow-Origin", "*"),
            new Tuple<string, string>("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept, Authorization"),
            new Tuple<string, string>("Access-Control-Allow-Methods", "POST, GET, PUT, DELETE")
        };
    }
}