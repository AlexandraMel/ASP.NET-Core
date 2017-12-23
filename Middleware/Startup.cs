using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace Middleware
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
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var time = DateTime.Now; 
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

            app.UseMiddleware<ExecuteTimeMiddleware>();

            app.Use(async (context, next) =>
            {
                await next.Invoke();
                var reqTime = DateTime.Now - time;
                await context.Response.WriteAsync($"\n Execute time class (lambda): {reqTime.Milliseconds}");
            });

            app.Use(async (context, n) =>
            {
                await context.Response.WriteAsync($"Request!");
            });
        }

        public class ExecuteTimeMiddleware
        {
            private readonly RequestDelegate _next;
            public DateTime date;

            public ExecuteTimeMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task InvokeAsync(HttpContext context)
            {
                var time = DateTime.Now;
                await _next.Invoke(context);
                var result =  DateTime.Now - time;
                await context.Response.WriteAsync($"\n Execute time (class): { result.Milliseconds}");
            }
        }
    }
}
