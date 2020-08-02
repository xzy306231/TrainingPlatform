using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using ApiUtil;
using ApiUtil.HttpApi;
using ApiUtil.Mq;
using AutoMapper;
using FreeSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PracticeManage.Entity;
using Steeltoe.Discovery.Client;
using Swashbuckle.AspNetCore.Swagger;
using WebApiClient.Extensions.DiscoveryClient;

namespace PracticeManage
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConfigUtil.InitConfig(configuration);
            ApiUtil.Entities.SystemLogEntity.ModuleName = "训练资源管理";
            Fsql = new FreeSql.FreeSqlBuilder()
                .UseConnectionString(DataType.MySql, ConfigUtil.MySqlConnectionString)
                .UseAutoSyncStructure(true)
                .UseLazyLoading(true)
                .UseMonitorCommand(cmd => Trace.WriteLine(cmd.CommandText))
                .Build();

            long count = 0;
            count = Fsql.Select<TaskEntity>().Count();
            count = Fsql.Select<SubjectEntity>().Count();
            count = Fsql.Select<TagEntity>().Count();
            count = Fsql.Select<SubjectBusEntity>().Count();
            count = Fsql.Select<SubjectTagRefEntity>().Count();
            count = Fsql.Select<SubjectBusTagRefEntity>().Count();

            ApiUtil.Entities.SystemLogEntity.ModuleName = "训练管理";
        }

        public IConfiguration Configuration { get; }
        public static IFreeSql Fsql { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDiscoveryClient(Configuration);//加载服务注册配置

            services.AddDiscoveryTypedClient<IPlatformApi>(c =>
            {
                c.HttpHost = new Uri(Configuration["EurekaService:Platform"]);
                c.FormatOptions.DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
            });//服务发现，平台服务

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


            services.AddSingleton<IFreeSql>(Fsql);
            services.AddAutoMapper(typeof(Startup));//Dto映射

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "训练管理API", Version = "v1" });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath, true);
                //c.OperationFilter<SwaggerFileUploadFilter>();
            });//Swagger

            services.AddSingleton<RabbitMqClient, RabbitMqClient>();//Rabbit 生产者
            services.AddSingleton<ServiceHelper, ServiceHelper>();
            services.AddSingleton<RedisUtil, RedisUtil>();
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "训练管理API V1");
                c.RoutePrefix = string.Empty;
            });
            app.UseMvc();
            app.UseDiscoveryClient();//启动服务注册
        }
    }
}
