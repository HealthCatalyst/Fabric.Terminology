namespace Fabric.Terminology.TestsBase
{
    using System.IO;

    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.TestsBase.Models;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class TerminologyTestHelper
    {
        public static FileInfo GetAppConfigFile()
        {
            var fileName = $"appsettings.json";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..", fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            return new FileInfo(filePath);
        }

        public static AppConfiguration GetAppConfig()
        {
            var file = GetAppConfigFile();

            return JsonConvert.DeserializeObject<AppConfiguration>(File.ReadAllText(file.FullName));
        }

        public static VersionConfig GetVersionJson()
        {
            var fileName = $"version.json";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\build\", fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            var file = new FileInfo(filePath);
            return JsonConvert.DeserializeObject<VersionConfig>(File.ReadAllText(file.FullName));
        }
    }
}