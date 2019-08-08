using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serializator;
using Sharing.DTO;
using System;
using System.Collections.Generic;

namespace MasterClient
{
    class MasterClient : IDisposable
    {
        private List<string> ClientsList = new List<string>();
        public IConnection connection;
        IModel channel;

        public MasterClient()
        {
            Logger.Info("Server started");
            ConnectionFactory factory = new ConnectionFactory() { UserName = "slavik", Password = "slavik", HostName = "127.0.0.1" };//port 15672
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: "MasterClient", durable: false, exclusive: false, autoDelete: false, arguments: null);
            //channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: "MasterClient", autoAck: true, consumer: consumer);
            consumer.Received += (model, ea) =>
            {
                IBasicProperties props = ea.BasicProperties;
                if (!ClientsList.Exists((c) => c == props.ReplyTo))
                {
                    ClientsList.Add(props.ReplyTo);
                }
                object obtained = ea.Body.Deserializer();
                switch (obtained)
                {
                    case CoordinatesRequest cr:
                        SendCoordinates(props.ReplyTo);
                        break;

                    default:
                        Logger.Error("Type is different!");
                        break;
                }
            };
        }

        private void SendCoordinates(string sessionQeueu)
        {
            CoordinatesResponse response = new CoordinatesResponse { Coordinates = ClientsList.ToArray() };
            channel.BasicPublish(exchange: "", routingKey: sessionQeueu, basicProperties: null, body: response.Serializer());
        }

        public void Dispose()
        {
            channel.Close();
            connection.Close();
        }
    }
}
