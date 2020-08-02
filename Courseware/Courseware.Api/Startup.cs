using System;
using System.IO;
using System.Reflection;
using ApiUtil;
using ApiUtil.Entities;
using ApiUtil.HttpApi;
using ApiUtil.Mq;
using AutoMapper;
using Courseware.Infrastructure.Database;
using Courseware.Infrastructure.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Steeltoe.Discovery.Client;
using Swashbuckle.AspNetCore.Swagger;
using WebApiClient.Extensions.DiscoveryClient;

namespace Courseware.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConfigUtil.InitConfig(configuration);
            SystemLogEntity.ModuleName = "课件管理";
            TodoEntity.TodoType = 2;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(option =>
            {
                option.AddPolicy("any", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            });//跨域

            services.AddDiscoveryClient(Configuration);//加载服务注册配置

            services.AddDiscoveryTypedClient<IPlatformApi>(c =>
            {
                c.HttpHost = new Uri(Configuration["EurekaService:Platform"]);
                c.FormatOptions.DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
            });//服务发现，平台服务
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddDbContextPool<MyContext>(option => option.UseMySQL(ConfigUtil.MySqlConnectionString), poolSize: 64);//数据库

            services.AddAutoMapper(typeof(Startup));//Dto映射

            services.AddTransient(typeof(UnitOfWork));//数据仓储

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "CourseResourceApi", Version = "v1" });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });//Swagger

            //services.AddSingleton<ILog, LogNLog>();//Rabbit NLog日志

            //services.AddHostedService<PersonInfoListener>();//全局监听RabbitMQ消息
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
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //if (env.IsDevelopment())
            //{
                
            //}
            //else if(env.IsProduction())
            //{
                
            //}
            //else if (env.IsStaging())
            //{
                
            //}
            //else if (env.IsEnvironment("xxx"))
            //{
                
            //}

            app.UseStaticFiles();//文件系统

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });
            //app.UseHttpsRedirection();//HTTP重定向
            app.UseMvc();
            app.UseDiscoveryClient();//启动服务注册

            app.UseCors("any");

        }
    }
}
