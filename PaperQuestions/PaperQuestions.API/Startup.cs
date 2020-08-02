using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaperQuestions.BLL;
using Steeltoe.Discovery.Client;
using Swashbuckle.AspNetCore.Swagger;

namespace PaperQuestions.API
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
            AddPaperQuestionsService(services);
            services.AddSingleton<RabbitMQClient>();
            string conn = Configuration.GetConnectionString("MySqlConnection");
            services.AddDbContext<pf_exam_paper_questionsContext>(arg =>
            {
                arg.UseMySql(conn);
                arg.EnableSensitiveDataLogging(true);
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "试卷库与题库管理", Version = "v1" });
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
                var xmlPath = Path.Combine(basePath, "PaperQuestions.API.xml");
                c.IncludeXmlComments(xmlPath,true);

            });
        }
        private void AddPaperQuestionsService(IServiceCollection services)
        {
            services.AddScoped<PaperQuestion>();
            services.AddScoped<Questions>();
            services.AddScoped<ExaminationPapers>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseDiscoveryClient();//启动服务注册

            //启用中间件服务生成Swagger作为JSON终结点
            app.UseSwagger();
            //启用中间件服务对swagger-ui，指定Swagger JSON终结点
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "试卷库与题库管理 V1");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
