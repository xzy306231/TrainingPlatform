using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;


public class RabbitMQClient
{
    private IConfiguration Configuration { get; }
    private readonly IModel _channel;
    string strExChange = string.Empty;
    string strRoutingKey1 = string.Empty;
    string strRoutingKey2 = string.Empty;
    string strRoutingKey3 = string.Empty;
    string strRoutingKey4 = string.Empty;
    public RabbitMQClient(IConfiguration configuration)
    {
        //从配置文件读取
        strExChange = configuration["RabbitMQ:Exchange"];
        strRoutingKey1 = configuration["RabbitMQ:RoutingKey1"];
        strRoutingKey2 = configuration["RabbitMQ:RoutingKey2"];
        strRoutingKey3 = configuration["RabbitMQ:RoutingKey3"];
        strRoutingKey3 = configuration["RabbitMQ:RoutingKey3"];
        //创建连接工厂
        var factory = new ConnectionFactory()
        {
            UserName = configuration["RabbitMQ:UserName"],
            Password = configuration["RabbitMQ:Password"],
            HostName = configuration["RabbitMQ:HostName"],
            Port = int.Parse(configuration["RabbitMQ:Port"]),
            VirtualHost = "/"
        };
        //创建连接
        var connection = factory.CreateConnection();
        //创建通道
        _channel = connection.CreateModel();
    }

    public void LogMsg(object message)
    {
        _channel.QueueDeclare(queue: "message",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
        string msgJson = JsonConvert.SerializeObject(message);

        var body = Encoding.UTF8.GetBytes(msgJson);
        _channel.BasicPublish(exchange: strExChange,
                                routingKey: strRoutingKey1,
                                basicProperties: null,
                                body: body);
    }
    public void ToDoMsg(object message)
    {
        _channel.QueueDeclare(queue: "message",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
        string msgJson = JsonConvert.SerializeObject(message);

        var body = Encoding.UTF8.GetBytes(msgJson);
        _channel.BasicPublish(exchange: strExChange,
                                routingKey: strRoutingKey2,
                                basicProperties: null,
                                body: body);
    }
    public void Msg(object message)
    {
        _channel.QueueDeclare(queue: "message",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
        string msgJson = JsonConvert.SerializeObject(message);

        var body = Encoding.UTF8.GetBytes(msgJson);
        _channel.BasicPublish(exchange: strExChange,
                                routingKey: strRoutingKey3,
                                basicProperties: null,
                                body: body);
    }
    public void ExamMsg(object message)
    {
        _channel.QueueDeclare(queue: "message",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
        string msgJson = JsonConvert.SerializeObject(message);

        var body = Encoding.UTF8.GetBytes(msgJson);
        _channel.BasicPublish(exchange: strExChange,
                                routingKey: strRoutingKey4,
                                basicProperties: null,
                                body: body);
    }
}

