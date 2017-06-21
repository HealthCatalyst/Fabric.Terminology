using System.IO;
using Fabric.Terminology.API.Configuration;
using Fabric.Terminology.SqlServer.Configuration;
using Newtonsoft.Json;

namespace Fabric.Terminology.TestsBase
{
    public static class TestHelper
    {
        public static FileInfo GetAppConfigFile()
        {
            var fileName = $"appsettings.json";
            var path = $"{Directory.GetCurrentDirectory()}\\..\\..\\..\\";
            var filePath = $"{path}{fileName}";

            return !File.Exists(filePath) ? null : new FileInfo(filePath);
        }

        public static AppConfiguration GetAppConfig()
        {
            var file = GetAppConfigFile();

            if (file == null)
            {
                return new AppConfiguration
                {
                    TerminologySqlSettings = new TerminologySqlSettings
                    {
                        UseInMemory = true
                    }
                };
            }

            return JsonConvert.DeserializeObject<AppConfiguration>(File.ReadAllText(file.FullName));
        }
    }

    public enum ConfigTestFor
    {
        Integration,
        Unit
    }
}