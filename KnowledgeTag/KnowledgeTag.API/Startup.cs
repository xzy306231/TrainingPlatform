using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Steeltoe.Discovery.Client;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.EntityFrameworkCore;
using KnowledgeTag.BLL;

namespace KnowledgeTag.API
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
            services.AddDiscoveryClient(Configuration);//加载服务注册配置
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddScoped<KnowledgeTagManagement>();
            string conn = Configuration.GetConnectionString("MySqlConnection");
            services.AddDbContext<pf_knowledge_tagContext>(arg =>
            {
                arg.UseMySql(conn);
                arg.EnableSensitiveDataLogging(true);
            });
            services.AddScoped<IHttpClientHelper, HttpClientHelper>();
            services.AddSingleton<RabbitMQClient>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "知识体系标签管理", Version = "v1" });
                c.OperationFilter<UpLoadFile>();
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
                var xmlPath = Path.Combine(basePath, "KnowledgeTag.API.xml");
                c.IncludeXmlComments(xmlPath);

            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseDiscoveryClient();//启动服务注册

            //启用中间件服务生成Swagger作为JSON终结点
            app.UseSwagger();
            //启用中间件服务对swagger-ui，指定Swagger JSON终结点
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "知识体系标签管理 V1");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
