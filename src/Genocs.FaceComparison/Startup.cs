using Genocs.FaceComparison.Options;
using Genocs.FaceComparison.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Genocs.FaceComparison
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
            services.AddApplicationInsightsTelemetry();

            services.AddOptions();
            services.Configure<AzureStorageConfig>(Configuration.GetSection("AzureStorageConfig"));
            services.Configure<AzureCognitiveServicesConfig>(Configuration.GetSection("AzureCognitiveServicesConfig"));

            services.AddSingleton<StorageService>();
            services.AddSingleton<FaceReconizerService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Genocs FaceComparison", Version = "v1", Description = "Face comparison service" });
            });

            #region Add CORS

            // Setup cors
            services.AddCors(options => options.AddPolicy("AllowAll", builder =>
                 builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()));
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Genocs FaceComparison v1"));

            // Enable Cors policy
            app.UseCors("AllowAll");

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
