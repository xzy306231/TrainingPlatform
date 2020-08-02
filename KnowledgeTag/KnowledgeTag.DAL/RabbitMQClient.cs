using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.IO;
using System.Text;

public class RabbitMQClient
{
    private readonly RabbitMQ.Client.IModel _channel;

    public RabbitMQClient()
    {
        var builder = new ConfigurationBuilder().SetBasePath(Path.GetDirectoryName(this.GetType().Assembly.Location)).AddJsonFile("dalsettings.json");
        var config = builder.Build();
        //从配置文件读取
        IConfigurationSection configUserName = config.GetSection("UserName");
        IConfigurationSection configPassWord = config.GetSection("Password");
        IConfigurationSection configHostName = config.GetSection("HostName");
        IConfigurationSection configPort = config.GetSection("Port");
        //创建连接工厂
        var factory = new ConnectionFactory()
        {
            UserName = configUserName.Value,
            Password = configPassWord.Value,
            HostName = configHostName.Value,
            Port =int.Parse(configPort.Value),
            VirtualHost = "/"
        };
        //创建连接
        var connection = factory.CreateConnection();
        //创建通道
        _channel = connection.CreateModel();
    }

    public virtual void PushMessage(object message)
    {
        string routingKey = "topic.logmsg";
        _channel.QueueDeclare(queue: "message",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
        string msgJson = JsonConvert.SerializeObject(message);

        var body = Encoding.UTF8.GetBytes(msgJson);
        _channel.BasicPublish(exchange: "topic_exchange",
                                routingKey: routingKey,
                                basicProperties: null,
                                body: body);

    }
}

