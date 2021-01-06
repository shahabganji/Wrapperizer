namespace Wrapperizer.Sample.Configurations
{
    public sealed class MongoDbConnection
    {
        public string Host { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 27017;
        public string Collection { get; set; }
        
        public string ConnectionString =>
            $"mongodb://{Host}:{Port}";
    }
}
