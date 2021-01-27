using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cw6.Middlewares;
using Cw6.Models;
using Cw6.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;


namespace Cw6
{
    public class Startup
    {
        private const string IndexHeader = "Index";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<ILogService, FileLogService>();
            services.AddScoped<IDbStudentService, MsSqlDbStudentService>();
            services.AddControllers();

            // Register the Swagger generator, defining Swagger documents
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Students API",
                    Version = "v1",
                    Description = "Students API prepared by Piotr Flis (s19841) for purposes of APB.",
                    Contact = new OpenApiContact { Name = "Piotr Flis s19841" }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbStudentService dbService)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "Students API"); });

            // Logging middleware is enabled after Swagger one to avoid logging requests for documentation.
            app.UseMiddleware<LogMiddleware>();

            app.Use(async (context, next) =>
            {
                if (!context.Request.Headers.ContainsKey(IndexHeader))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("No headline with index number");
                    return;
                }

                var index = context.Request.Headers[IndexHeader].ToString();
                if (dbService.GetStudent(index) == null)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsync("Index number not found in database");
                    return;
                }

                await next();
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}