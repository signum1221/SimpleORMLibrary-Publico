using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleORMLibrary.Databases;
using SimpleORMLibrary.Models;

namespace WebTFG
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            //Loading SimpleORMLibrary Data
            DatabaseManager.loadDatabases("C:\\Users\\Signum\\Desktop\\Entrega TFG\\Codigo\\WebTFG\\Models\\SimpleORMLibraryMaps\\DatabaseFile.xml");
            ModelManager.addModels("C:\\Users\\Signum\\Desktop\\Entrega TFG\\Codigo\\WebTFG\\Models\\SimpleORMLibraryMaps\\Producto.xml");
            ModelManager.addModels("C:\\Users\\Signum\\Desktop\\Entrega TFG\\Codigo\\WebTFG\\Models\\SimpleORMLibraryMaps\\LineaCarro.xml");
            ModelManager.addModels("C:\\Users\\Signum\\Desktop\\Entrega TFG\\Codigo\\WebTFG\\Models\\SimpleORMLibraryMaps\\Carro.xml");
            ModelManager.addModels("C:\\Users\\Signum\\Desktop\\Entrega TFG\\Codigo\\WebTFG\\Models\\SimpleORMLibraryMaps\\LineaFactura.xml");
            ModelManager.addModels("C:\\Users\\Signum\\Desktop\\Entrega TFG\\Codigo\\WebTFG\\Models\\SimpleORMLibraryMaps\\Factura.xml");
            ModelManager.addModels("C:\\Users\\Signum\\Desktop\\Entrega TFG\\Codigo\\WebTFG\\Models\\SimpleORMLibraryMaps\\Usuario.xml");

            //WebTFG.Models.Producto.debugRegistraProductos();
            //WebTFG.Models.Producto.debugRegistraProductosYMideTiempo();
           
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseSession();
            app.UseMvc(routes =>
            {

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Inicio}/{action=Inicio}/{*url}");
                
                routes.MapRoute(
                    name: "NotFound",
                    template: "{*url}",
                    defaults: new { controller = "Inicio", action = "Error" }
                );
            });
        }
    }
}
