//using System;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json.Linq;

//namespace ApiUtil.Mq
//{
//    public class PersonInfoListener : RabbitListener
//    {

//        private readonly ILogger<RabbitListener> _logger;

//        // 因为Process函数是委托回调,直接将其他Service注入的话两者不在一个scope,
//        // 这里要调用其他的Service实例只能用IServiceProvider CreateScope后获取实例对象
//        private readonly IServiceProvider _services;

//        public PersonInfoListener(IServiceProvider services, ILogger<RabbitListener> logger, IConfiguration configuration):base(configuration)
//        {
//            RouteKey = "personInfo";
//            QueueName = "personInfoListener";
//            ExchangeName = "PersonInfo.Exchange";
//            _logger = logger;
//            _services = services;
//        }

//        public override bool Process(string message)
//        {
//            //json对象转换
//            //处理完成返回true
//            var taskMessage = JToken.Parse(message);
//            if (taskMessage == null)
//            {
//                // 返回false 的时候回直接驳回此消息,表示处理不了
//                return false;
//            }
//            try
//            {
//                using (var scope = _services.CreateScope())
//                {
//                    //var xxxService = scope.ServiceProvider.GetRequiredService<XXXXService>();
//                    return true;
//                }

//            }
//            catch (Exception ex)
//            {
//                _logger.LogInformation($"Process fail,error:{ex.Message},stackTrace:{ex.StackTrace},message:{message}");
//                _logger.LogError(-1, ex, "Process fail");
//                return false;
//            }
//        }
//    }
//}
