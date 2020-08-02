using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Steeltoe.Discovery.Client;
using Swashbuckle.AspNetCore.Swagger;


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
        services.AddDbContext<pf_training_plan_v1Context>(arg =>
        {
            arg.UseMySql(conn);
            arg.EnableSensitiveDataLogging(true);
        });
       // services.AddDbContextPool<pf_training_plan_v1Context>(arg => arg.UseMySql(conn),64);
        AddTrainingPlanService(services);
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Info
            {
                Title = "培训系统",
                Version = "v1",
                Description = "描述类信息",
                Contact = new Contact { Name = "登录网址", Email = "982913833@qq.com", Url = "http://192.168.1.149/puxuweb/#/" }
            });
            var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            var xmlPath = Path.Combine(basePath, "TrainingPlan.API.xml");
            c.IncludeXmlComments(xmlPath, true);

        });
       
    }

    private void AddTrainingPlanService(IServiceCollection services)
    {
        services.AddScoped<EffectEvaluation>();
        services.AddScoped<MyCourse>();
        services.AddScoped<MyTraining>();
        services.AddScoped<MyTrainingTask>();
        services.AddScoped<RemoteService>();
        services.AddScoped<StatisticData>();
        services.AddScoped<TrainingPlan>();
        services.AddScoped<CourseManagement>();
        services.AddScoped<LearningMap>();
        services.AddScoped<LessonSchedule>();
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
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "培训系统 V1");
            c.RoutePrefix = string.Empty;
        });
       
    }
}

