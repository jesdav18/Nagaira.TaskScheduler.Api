using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.OpenApi.Models;
using Nagaira.TaskScheduler.Api.Helpers;
using Nagaira.TaskScheduler.Api.Service;
using Nagaira.TaskScheduler.Api.Service.Executers;

namespace Nagaira.TaskScheduler.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private void RegistrarJobsExecuters(IServiceCollection services)
        {
            services.AddTransient<TaskManagementAppService>();
            services.AddTransient<IJobExecuter, HttpJobExecuterAppService>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            RegistrarJobsExecuters(services);
            services.AddTransient<HttpClient>();

            string conexionHangFire = _configuration!.GetConnectionString("HangFire")!;

            services.AddHangfire(configuration => configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UsePostgreSqlStorage(options =>
                    {
                        options.UseNpgsqlConnection(conexionHangFire);
                    })
            );

            services.AddScoped(x =>
            {
                return new GlobalEnvironmentDto
                {
                    TimeZone = _configuration.GetSection("General")["TimeZone"] ?? "0",
                    TimeoutInMinutes = _configuration.GetSection("General")["TimeoutInMinutes"] ?? "0",
                };
            });

            services.AddHangfireServer(optionsServe => optionsServe.WorkerCount = 10);

            services.AddMvcCore().AddNewtonsoftJson();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Nagaira.TaskScheduler.Api", Version = "v1" });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("DevelopmentCors",
                    builder => builder
                    .SetIsOriginAllowed(_ => true).AllowAnyHeader().AllowAnyMethod().AllowCredentials());

                options.AddPolicy("ProductionCors",
                builder => builder.WithOrigins("")
                                  .AllowAnyMethod()
                                  .AllowAnyHeader()
                                  .AllowCredentials());

            });
        }

        public class NagairaFilter : IDashboardAuthorizationFilter
        {
            public bool Authorize(DashboardContext context) => true;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var corsEnvironment = "DevelopmentCors";

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                corsEnvironment = "DevelopmentCors";
            }

            var varMonitoringApi = JobStorage.Current.GetMonitoringApi();
            var varServerList = varMonitoringApi.Servers();
            foreach (var varServerItem in varServerList)
            {
                JobStorage.Current.GetConnection().RemoveServer(varServerItem.Name);
            }

            var options = new DashboardOptions()
            {
                Authorization = new[] { new NagairaFilter() }
            };

            app.UseHangfireDashboard("/hangfire", options);

            app.UseCors(corsEnvironment);
            app.UseRouting();
            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Nagaira.TaskScheduler.Api V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    var ambiente = env.IsProduction() ? "Producción" : "Desarrollo";
                    await context.Response.WriteAsync($"<b>Nagaira TaskScheduler Api | {ambiente}</b><hr>");
                });
                endpoints.MapControllers();
            });

        }
    }
}
