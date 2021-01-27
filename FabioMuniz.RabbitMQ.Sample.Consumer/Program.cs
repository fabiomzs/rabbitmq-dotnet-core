using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace FabioMuniz.RabbitMQ.Sample.Consumer
{
    static class Program
    {
        static void Main(string[] args)
        {
            RabbitMQConsumer();
        }

        static void RabbitMQConsumer()
        {
            string queue = "order-sample-copy";

            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "rabbit",
                Password = "rabbittest"
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queue, durable: false, exclusive: false, autoDelete: false, arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += Consumer_Received;

                    channel.BasicConsume(queue: queue, autoAck: true, consumer: consumer);

                    Console.ReadKey();
                }
            }
        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("MESSAGE RECEIVED: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message} ---> {ex?.InnerException?.Message}");
            }
        }
    }
}
