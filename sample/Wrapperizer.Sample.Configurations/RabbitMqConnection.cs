using System;

namespace Wrapperizer.Sample.Configurations
{
    public sealed class RabbitMqConnection
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string UserId { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "";

        public string ConnectionString => $"amqp://{UserId}:{Password}@{Host}:{Port}/{VirtualHost}";
        public Uri ConnectionUri => new Uri(ConnectionString);
    }
}