using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CartRepository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CartService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            string path = Environment.CurrentDirectory+"/../CartDb.sqlite";
            string connectionString = $"Data Source={path}";
            //"Server=.\\SQLEXPRESS;Initial Catalog=cartstore;Integrated Security=True";
            services.AddTransient<IBaseRepository, BaseRepository>(provider =>
                new BaseRepository(connectionString));
            services.AddTransient<ICartRepository, CartRepository.CartRepository>(provider =>
            {
                var rep =new CartRepository.CartRepository(connectionString);
                if (!File.Exists(path))
                {
                    rep.CreateDatabase();
                }

                return rep;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}