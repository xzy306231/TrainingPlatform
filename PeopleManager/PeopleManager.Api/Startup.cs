using System;
using System.IO;
using System.Reflection;
using ApiUtil;
using ApiUtil.HttpApi;
using ApiUtil.Mq;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using PeopleManager.Api.Common;
using PeopleManager.Infrastructure.Database;
using PeopleManager.Infrastructure.Repository;
using Steeltoe.Discovery.Client;
using Swashbuckle.AspNetCore.Swagger;
using WebApiClient.Extensions.DiscoveryClient;

namespace PeopleManager.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConfigUtil.InitConfig(configuration);
            ApiUtil.Entities.SystemLogEntity.ModuleName = "教学员管理";
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(option =>
            {
                option.AddPolicy("any",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            });//跨域

            services.AddDiscoveryClient(Configuration);//加载服务注册配置

            services.AddDiscoveryTypedClient<IPlatformApi>(c =>
            {
                c.HttpHost = new Uri(Configuration["EurekaService:Platform"]);
                c.FormatOptions.DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
            });//服务发现，平台服务

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));
            
            services.AddDbContextPool<MyContext>(options => options.UseMySQL(ConfigUtil.MySqlConnectionString), poolSize: 64);//数据库

            services.AddAutoMapper(typeof(Startup));//Dto映射

            services.AddTransient(typeof(UnitOfWork));//数据仓储

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "人员管理API", Version = "v1" });//OpenApiInfo {Title = "人员管理API", Version = "v1"}
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath,true);
                c.OperationFilter<SwaggerFileUploadFilter>();
            });//Swagger

            services.AddSingleton<RabbitMqClient, RabbitMqClient>();//Rabbit 生产者
            services.AddSingleton<ServiceHelper, ServiceHelper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            ConfigUtil.ServiceProvider = app.ApplicationServices;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "人员管理API V1");
                c.RoutePrefix = string.Empty;
            });
            app.UseStaticFiles();
            app.UseMvc();
            app.UseDiscoveryClient();//启动服务注册

            app.UseCors("any");
        }
    }
}
