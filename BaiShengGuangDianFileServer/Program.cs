using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BaiShengGuangDianFileServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
#if DEBUG
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
#else
            var configuration = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            return WebHost.CreateDefaultBuilder(args).UseConfiguration(configuration)
                .UseStartup<Startup>();
#endif
        }
    }
}
