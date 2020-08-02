using System.IO;
using Examination.BLL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Steeltoe.Discovery.Client;
using Swashbuckle.AspNetCore.Swagger;

namespace Examination.API
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
            services.AddScoped<IHttpClientHelper, HttpClientHelper>();
            services.AddSingleton<RabbitMQClient>();
            string conn = Configuration.GetConnectionString("MySqlConnection");
            services.AddDbContext<pf_examinationContext>(arg =>
            {
                arg.UseMySql(conn);
                arg.EnableSensitiveDataLogging(true);
            });
            AddExaminationService(services);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "考试管理", Version = "v1" });
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
                var xmlPath = Path.Combine(basePath, "Examination.API.xml");
                c.IncludeXmlComments(xmlPath);

            });
        }
        private void AddExaminationService(IServiceCollection services)
        {
            services.AddScoped<CorrectExamPaper>();
            services.AddScoped<EffectEvaluation>();
            services.AddScoped<ExaminationManage>();
            services.AddScoped<MyExamination>();
            services.AddScoped<RemoteService>();
            services.AddScoped<StatisticData>();
            services.AddScoped<TestPaperInfo>();
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "考试管理 V1");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
