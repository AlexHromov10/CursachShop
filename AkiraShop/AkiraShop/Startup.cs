using AkiraShop.Data;
using AkiraShop.Data.Interfaces;
using AkiraShop.Data.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AkiraShop
{
    public class Startup
    {
        private IConfigurationRoot _configurationString;

        public Startup(IWebHostEnvironment hostEnv)
        {
            _configurationString = new ConfigurationBuilder().SetBasePath(hostEnv.ContentRootPath).AddJsonFile("dbsettings.json").Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IAllItems, ItemsRepository>();
            services.AddTransient<IItemsCategory, CategoryRepository>();// —¬ﬂ« ¿ ÃŒƒ≈À»  » »Õ≈“–‘≈…—¿
            //services.AddScoped<IItemsCategory, CategoryRepository>();

            services.AddMvc();
            services.AddMvc(option => option.EnableEndpointRouting = false);

            services.AddEntityFrameworkNpgsql().AddDbContext<AppDBContent>(opt => opt.UseNpgsql(_configurationString.GetConnectionString("MyWebApiConnection")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
            var contentRoot = Directory.GetCurrentDirectory();
            env.WebRootPath = Path.Combine(contentRoot, "wwwroot");
            //dbObjects.Initial(app);
        }
    }
}
