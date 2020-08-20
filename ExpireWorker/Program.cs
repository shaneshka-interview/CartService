using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CartRepository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExpireWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    string connectionString = Settings.Settings.ConnectionString;

                    services.AddTransient<IBaseRepository, BaseRepository>(provider =>
                        new BaseRepository(connectionString));

                    services.AddTransient<ICartsRepository, CartsRepository>(provider =>
                        new CartsRepository(connectionString));

                    services.AddHostedService<Worker>();
                });
    }
}