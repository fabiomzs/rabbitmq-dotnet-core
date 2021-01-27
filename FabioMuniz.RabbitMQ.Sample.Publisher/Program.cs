using FabioMuniz.RabbitMQ.Sample.Domain;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;

namespace FabioMuniz.RabbitMQ.Sample.Publisher
{
    static class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();
            Order order;

            for (int i = 0; i < 200; i++)
            {
                order = new Order(random.Next(1, 10), random.Next(100, 1200), "Fabio Muniz");

                RabbitMQPublish(JsonConvert.SerializeObject(order));

                Thread.Sleep(500);
            }
        }

        static void RabbitMQPublish(string message)
        {
            string queue = "order-sample";

            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "rabbit",
                Password = "rabbittest"
            };

            try
            {
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: queue, durable: false, exclusive: false, autoDelete: false, arguments: null);

                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "", routingKey: queue, basicProperties: null, body: body);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("MESSAGE SENT: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(message);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message} ---> {ex?.InnerException?.Message}");
            }
        }
    }
}
