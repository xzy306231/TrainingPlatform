using CoursewareDev.BLL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Steeltoe.Discovery.Client;
using System.IO;

namespace CoursewareDev.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDiscoveryClient(Configuration);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            string conn = Configuration.GetConnectionString("MySqlConnection");
            services.AddDbContext<pf_courseware_devContext>(arg =>
            {
                arg.UseMySql(conn);
                arg.EnableSensitiveDataLogging(true);
            });
            services.AddScoped<IHttpClientHelper, HttpClientHelper>();
            services.AddScoped<Courseware>();
            services.AddScoped<RemoteService>();
            services.AddSingleton<RabbitMQClient>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "课件开发",
                    Version = "v1",
                    Description = "Description",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Email = "982913833@qq.com",
                        Name = "Mr Xu"
                        // Url =new System.Uri("")
                    }
                });
                var basePath = Path.GetDirectoryName(typeof(Startup).Assembly.Location);
                var xmlPath = Path.Combine(basePath, "CoursewareDev.API.xml");
                c.IncludeXmlComments(xmlPath, true);
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //启用中间件服务生成Swagger作为JSON终结点
            app.UseSwagger();
            //启用中间件服务对swagger-ui，指定Swagger JSON终结点
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "课件开发 V1");
                c.RoutePrefix = string.Empty;
            });
            app.UseMvc();
            app.UseDiscoveryClient();//启动服务注册
        }
    }
}
