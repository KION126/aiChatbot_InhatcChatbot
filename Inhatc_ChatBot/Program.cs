// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.22.0

using Inhatc_Chatbot;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Inhatc_ChatBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebCrawling.crawling();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
