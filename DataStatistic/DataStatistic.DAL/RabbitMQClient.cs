using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.IO;
using System.Text;

namespace DataStatistic.DAL
{
    public class RabbitMQClient
    {
        private readonly RabbitMQ.Client.IModel _channel;
        string strExChange = string.Empty;
        string strRoutingKey1 = string.Empty;
        string strRoutingKey2 = string.Empty;
        string strRoutingKey3 = string.Empty;
        public RabbitMQClient()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Path.GetDirectoryName(this.GetType().Assembly.Location)).AddJsonFile("dalsettings.json");
            var config = builder.Build();
            //从配置文件读取
            IConfigurationSection configUserName = config.GetSection("UserName");
            IConfigurationSection configPassWord = config.GetSection("Password");
            IConfigurationSection configHostName = config.GetSection("HostName");
            IConfigurationSection configPort = config.GetSection("Port");
            strExChange = config.GetSection("Exchange").Value;
            strRoutingKey1 = config.GetSection("RoutingKey1").Value;
            strRoutingKey2 = config.GetSection("RoutingKey2").Value;
            strRoutingKey3 = config.GetSection("RoutingKey3").Value;

            //创建连接工厂
            var factory = new ConnectionFactory()
            {
                UserName = configUserName.Value,
                Password = configPassWord.Value,
                HostName = configHostName.Value,
                Port = int.Parse(configPort.Value),
                VirtualHost = "/"
            };
            //创建连接
            var connection = factory.CreateConnection();
            //创建通道
            _channel = connection.CreateModel();
        }

        public virtual void PushMessage(object message, int i)
        {
            string routingKey = string.Empty;
            if (i == 1)//日志消息
            {
                routingKey = strRoutingKey1;
            }
            else if (i == 2)//待办消息
            {
                routingKey = strRoutingKey2;
            }
            else if (i == 3)
            {
                routingKey = strRoutingKey3;
            }

            _channel.QueueDeclare(queue: "message",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);
            string msgJson = JsonConvert.SerializeObject(message);

            var body = Encoding.UTF8.GetBytes(msgJson);
            _channel.BasicPublish(exchange: strExChange,
                                    routingKey: routingKey,
                                    basicProperties: null,
                                    body: body);

        }
    }
}
