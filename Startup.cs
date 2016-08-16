using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApplication4.Models;

namespace WebApplication4
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            using (var db = new UserContext())
            {
                db.Database.EnsureCreated();
            }

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFrameworkSqlite().AddDbContext<UserContext>();

            // Add framework services.
            services.AddMvc();
        }
        //
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            //

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();



            //вызов компонента middleware, который осуществляет аутентификацию по куки
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                //название схемы аутентификации - случайное название, которое потом будет использоваться для аутентификации пользователя
                AuthenticationScheme = "Cookies",
                //устанавливает относительный путь, по которому будет перенаправляться анонимный пользователь при доступе к ресурсам, для которых нужна аутентификация
                LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login"),
                //при значении true компонент UseCookieAuthentication будет при каждом запросе запускаться и проверять пользователя на аутентификацию
                AutomaticAuthenticate = true,
                //если это свойство равно true, то не авторизованный пользователь при попытке доступа к ресурсам, для которых нужна авторизация, будет перенаправляться по пути в свойстве LoginPath 
                AutomaticChallenge = true 
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });



        }
    }
}
