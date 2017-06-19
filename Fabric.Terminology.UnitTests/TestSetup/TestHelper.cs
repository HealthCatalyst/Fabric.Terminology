using System;
using System.IO;

namespace Fabric.Terminology.UnitTests.TestSetup
{
    internal static class TestHelper
    {
        public static FileInfo GetConfigFile(ConfigTestFor testType = ConfigTestFor.Unit)
        {
            var fileName = $"appsettings.{testType.ToString()}.json";
            var path = $"{Directory.GetCurrentDirectory()}\\..\\..\\..\\App_Data\\Configs\\";
            var filePath = $"{path}{fileName}";

            if (!File.Exists(filePath)) throw new NullReferenceException("Could not find configuration file.");

            return new FileInfo(filePath);
        }
    }

    public enum ConfigTestFor
    {
        Integration,
        Unit
    }
}