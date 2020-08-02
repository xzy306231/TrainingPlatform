using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ApiUtil.Mq
{
    /// <summary>
    /// RabbitMq监听
    /// </summary>
    public abstract class RabbitListener : IHostedService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        /// <summary>
        /// 路由
        /// </summary>
        protected string RouteKey { get; set; }

        /// <summary>
        /// 队列
        /// </summary>
        protected string QueueName;

        /// <summary>
        /// 交换机
        /// </summary>
        protected string ExchangeName { get; set; } = "rmq.target.demo";

        /// <summary>
        /// 交换机使用的模式
        /// </summary>
        protected string ExchangeTypeName { get; set; } = ExchangeType.Direct;

        /// <summary>
        /// TODO：后续需要把配置文件导入
        /// </summary>
        protected RabbitListener(IConfiguration configuration)
        {
            try
            {
                //创建连接工厂
                var factory = new ConnectionFactory()
                {
                    UserName = configuration["RabbitMQ:UserName"],//用户名
                    Password = configuration["RabbitMQ:Password"],//密码
                    HostName = configuration["RabbitMQ:HostName"],//rabbitmq ip //114.67.109.248
                    Port = Convert.ToInt32(configuration["RabbitMQ:Port"]),
                };
                //创建连接
                _connection = factory.CreateConnection();
                //创建通道
                _channel = _connection.CreateModel();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Register();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _connection.Close();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 处理消息的方法
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public abstract bool Process(string message);

        /// <summary>
        /// 注册消费者监听在这里
        /// </summary>
        public void Register()
        {
            Console.WriteLine($"RabbitListener register, routeKey:{RouteKey}");
            //声明交换机
            _channel.ExchangeDeclare(
                exchange: ExchangeName, 
                type: ExchangeTypeName);
            //声明一个队列
            _channel.QueueDeclare(
                queue: QueueName, 
                exclusive: false);
            //绑定队列，交换机，路由键
            _channel.QueueBind(
                queue: QueueName, 
                exchange: ExchangeName, 
                routingKey: RouteKey);

            var basicProperties = _channel.CreateBasicProperties();
            //1：非持久化 2：可持久化
            basicProperties.DeliveryMode = 2;

            //事件基本消费者
            var consumer = new EventingBasicConsumer(_channel);
            //接收到消息事件
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                var result = Process(message);
                if (result)
                {
                    //确认该消息已被消费
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
            };
            //启动消费者 设置为手动应答消息
            _channel.BasicConsume(queue: QueueName, consumer: consumer);
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeRegister()
        {
            _connection.Close();
        }
    }
}
