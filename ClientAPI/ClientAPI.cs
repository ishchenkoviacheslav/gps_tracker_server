using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serializator;
using Sharing.DTO;
using System;

namespace ClientAPI
{
    public class ClientAPI : IDisposable
    {
        public event Action<CoordinatesResponse> UpdateCoordinates;
        private IConnection connection;
        private IModel channel;
        private readonly string queueName = Guid.NewGuid().ToString();
        private EventingBasicConsumer consumer;
        private IBasicProperties props;

        public ClientAPI()
        {
            ConnectionFactory factory = new ConnectionFactory() { UserName = "slavik", Password = "slavik", HostName = "95.46.44.84" };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: true, arguments: null);
            //replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);

            props = channel.CreateBasicProperties();
            props.ReplyTo = queueName;
            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            consumer.Received += (model, ea) =>
            {
                object obtained = ea.Body.Deserializer();
                switch (obtained)
                {
                    case CoordinatesResponse cr:
                        UpdateCoordinates?.Invoke(cr);
                        break;
                    default:
                        throw new Exception("Type if different! Server sent unknown type!");
                }
            };
        }

        public void GetCoordinates(CoordinatesRequest request)
        {
            channel.BasicPublish(exchange: "", routingKey: "MasterClient", basicProperties: props, body: request.Serializer());
        }

        public void Dispose()
        {
            channel.Close();
            connection.Close();
        }
    }
}
