using System.IO;
using Microsoft.Extensions.Configuration;

namespace TestSystem.Data.Tests
{
    internal class Configuration
    {
        static Configuration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            ConfigurationRoot = builder.Build();
        }

        public static IConfigurationRoot ConfigurationRoot { get; }

        public static string Get(string key) => ConfigurationRoot[key];
    }
}