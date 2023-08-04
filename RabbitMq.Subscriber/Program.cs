using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace RabbitMq.Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://kfsdoqic:jn0RezxxesWFhHedSXKfrxlcieC0PsC_@fish.rmq.cloudamqp.com/kfsdoqic");
            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();
        
    
            channel.BasicQos(0, 1, false);
            var subscriber = new EventingBasicConsumer(channel);

            var queueName = "direct-queue - Error";
            Console.WriteLine("Loglar dinleniyor ..");


            subscriber.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Thread.Sleep(1500);
                Console.WriteLine("Gelen mesaj : " + message);

                channel.BasicAck(e.DeliveryTag, false);
            };

            channel.BasicConsume(queueName, false, subscriber);

            Console.ReadLine();
        }


    }
}



