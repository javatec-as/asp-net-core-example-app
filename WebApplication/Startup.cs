using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using WebApplication.Middleware;
using WebApplication.Nhibernate;
using WebApplication.Repository;
using Environment = NHibernate.Cfg.Environment;

namespace WebApplication
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
            services.AddSingleton(provider =>
            {
                var configuration = new Configuration();

                configuration.SetProperties(new Dictionary<string, string>
                {
                    {Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider"},
                    {Environment.Dialect, "NHibernate.Dialect.SQLiteDialect"},
                    {Environment.ConnectionDriver, "NHibernate.Driver.SQLite20Driver"},
                    {Environment.ConnectionString, Configuration.GetConnectionString("DefaultConnection")},
                    {Environment.CurrentSessionContextClass, "WebApplication.Nhibernate.AspNetCoreWebSessionContext, WebApplication"},
                    {Environment.ShowSql, "true"}
                });
                configuration.AddClass(typeof(Customer));
                configuration.AddClass(typeof(Contact));

                new SchemaExport(configuration).Execute(true, true, false);
                return configuration.BuildSessionFactory();
            });

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<Func<IHttpContextAccessor>>(provider => provider.GetService<IHttpContextAccessor>);

            services.AddSingleton<ISessionManager, SessionManager>();
            services.AddSingleton<ICustomerRepo, CustomerRepo>();

            services.AddRazorPages(options =>
            {
                options.Conventions.AllowAnonymousToPage("/Login");
                options.Conventions.AllowAnonymousToPage("/Error");
            });

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.Cookie.Name = ".AspNet.SharedCookie";
                    options.Cookie.Path = "/";
                    options.LoginPath = "/Login";
                    options.LogoutPath = "/Login";
                    options.AccessDeniedPath = "/Error";
                });

            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseWhen(context => !context.Request.Path.ToString().Contains("."), appBuilder => appBuilder
                .UseNhibernateMiddleware());

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
