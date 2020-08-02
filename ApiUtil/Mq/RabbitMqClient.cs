using System;
using System.Text;
using ApiUtil.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace ApiUtil.Mq
{
    /// <summary>
    /// RabbitMq生产者
    /// </summary>
    public class RabbitMqClient
    {
        private readonly IModel _channel;

        private readonly ILogger _logger;

        #region ::::: 配置字段 :::::

        private readonly string _exchangeName;

        private readonly string _fileRouting;

        private readonly string _logRouting;

        private readonly string _msgRouting;

        private readonly string _scormRouting;

        private readonly string _todoRouting;

        private readonly string _userMsgRouting;

        private readonly string _videoRouting;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string LocalIp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public RabbitMqClient(ILogger<RabbitMqClient> logger,IConfiguration configuration)
        {
            //
            _exchangeName = configuration["RabbitMQ:Exchange"];
            _fileRouting = configuration["RabbitMQ:file_routing"];
            _logRouting = configuration["RabbitMQ:logmsg_routing"];
            _msgRouting = configuration["RabbitMQ:msg_routing"];
            _scormRouting = configuration["RabbitMQ:scormmsg_routing"];
            _todoRouting = configuration["RabbitMQ:todomsg_routing"];
            _userMsgRouting = configuration["RabbitMQ:usermsg_routing"];
            _videoRouting = configuration["RabbitMQ:video_routing"];
            LocalIp = configuration["eureka:instance:hostName"];
            try
            {
                //创建连接工厂
                var factory = new ConnectionFactory
                {
                    UserName = configuration["RabbitMQ:UserName"],//用户名
                    Password = configuration["RabbitMQ:Password"],//密码
                    HostName = configuration["RabbitMQ:HostName"],//rabbitmq ip //114.67.109.248
                    Port = Convert.ToInt32(configuration["RabbitMQ:Port"]),
                    VirtualHost = configuration["RabbitMQ:VirtualHost"]
                };
                //创建连接
                var connection = factory.CreateConnection();
                //创建通道
                _channel = connection.CreateModel();
                //_channel.ExchangeDeclare(exchange:_exchangeName,type:"topic");
            }
            catch (Exception ex)
            {
                logger.LogError(-1, ex, "RabbitMQClient init fail");
            }
            _logger = logger;
        }

        /// <summary>
        /// 完整消息发送
        /// </summary>
        /// <param name="exchangeName">交换机</param>
        /// <param name="routingKey">路由名</param>
        /// <param name="message">消息体</param>
        public virtual void PushMessage(string exchangeName, string routingKey, object message)
        {
            _logger.LogInformation($"PushMessage,routingKey:{routingKey}");
            //声明
            //_channel.QueueDeclare(queue: "message",durable: false,exclusive: false,autoDelete: false,arguments: null);
            string msgJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(msgJson);
            try
            {
                _channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, basicProperties: null, body: body);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="routingKey"></param>
        /// <param name="message"></param>
        public virtual void PushMessage(string routingKey, object message)
        {
            PushMessage(_exchangeName, routingKey, message);
        }

        /// <summary>
        /// 文件上传发送消息
        /// </summary>
        /// <param name="message"></param>
        public void PushFileMessage(object message)
        {
            PushMessage(_fileRouting, message);
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="message"></param>
        public void PushLogMessage(SystemLogEntity message)
        {
            PushMessage(_logRouting, message);
        }

        /// <summary>
        /// 服务间消息
        /// </summary>
        /// <param name="message"></param>
        public void PushMsgMessage(MessageEntity message)
        {
            PushMessage(_msgRouting, message);
        }

        /// <summary>
        /// SCORM课件消息
        /// </summary>
        /// <param name="message"></param>
        public void PushScormMessage(object message)
        {
            PushMessage(_scormRouting, message);
        }

        /// <summary>
        /// 待办消息
        /// </summary>
        /// <param name="message"></param>
        public void PushTodoMessage(TodoEntity message)
        {
            PushMessage(_todoRouting, message);
        }

        /// <summary>
        /// 用户信息
        /// </summary>
        /// <param name="message"></param>
        public void PushUserMessage(object message)
        {
            PushMessage(_userMsgRouting, message); 
        }

        /// <summary>
        /// 视频消息
        /// </summary>
        /// <param name="message"></param>
        public void PushTransformMessage(FileTransfEntity message)
        {
            PushMessage(_videoRouting, message);
        }
    }
}
