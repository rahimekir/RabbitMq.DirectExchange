using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace RabbitMq.Publisher
{

    public enum LogNames
    {
        Critical = 1,
        Error = 2,
        Warning = 3,
        Info = 4

    }
    class Program
    {

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://kfsdoqic:jn0RezxxesWFhHedSXKfrxlcieC0PsC_@fish.rmq.cloudamqp.com/kfsdoqic");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();


            channel.ExchangeDeclare("logs-direct", type: ExchangeType.Direct, durable: true);  //=>Exchange oluşturma işlemi

           
            Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
            {
                //her enum için kuyruk oluşturma işlemi
                var queueName = $"direct-queue - {x}" ;
                
                channel.QueueDeclare(queueName, true, false, false);
                //routeKey belirleme
                var routeKey = $"route-{x}";

                channel.QueueBind(queueName,"logs-direct", routeKey,null); // routekeylerin ilgili  kuyruğa set edilmesi için 

            });

            
            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                LogNames log = (LogNames)new Random().Next(1, 5);

                string message = $"log-type {log} ";
                var messageBody = Encoding.UTF8.GetBytes(message);

                //routeKey belirleme
                var routeKey = $"route-{log}";


                //channel.BasicPublish(string.Empty, string routingkey, null, messageBody); => eskisi
                channel.BasicPublish("logs-direct", routeKey, null, messageBody);  //eskisinde direkt kuyruğa gönderdiğimiz içi  kuyruğun ismini vermiştik burda boş kalacak.

                Console.WriteLine($"Log gönderilmiştir.: {message}");
            }
            );

            Console.ReadLine();
        }
    }
}
