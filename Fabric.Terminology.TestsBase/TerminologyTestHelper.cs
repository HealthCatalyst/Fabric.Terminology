namespace Fabric.Terminology.TestsBase
{
    using System.IO;

    using Fabric.Terminology.API.Configuration;

    using Newtonsoft.Json;

    public static class TerminologyTestHelper
    {
        public static FileInfo GetAppConfigFile()
        {
            var fileName = $"appsettings.json";
            var path = $"{Directory.GetCurrentDirectory()}\\..\\..\\..\\";
            var filePath = $"{path}{fileName}";

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
    }
}