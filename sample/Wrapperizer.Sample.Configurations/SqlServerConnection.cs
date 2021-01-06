namespace Wrapperizer.Sample.Configurations
{
    public sealed class SqlServerConnection
    {
        public string Server { get; set; } = "localhost";
        public string Database { get; set; }
        public int Port { get; set; } = 1433;
        public string UserId { get; set; }
        public string Password { get; set; }
        public bool MultipleActiveResultSet { get; set; } = true;

        public string ConnectionString =>
            $"Server={Server},{Port}; Database={Database}; " +
            $"UID={UserId}; PWD={Password};" +
            $"MultipleActiveResultSets={MultipleActiveResultSet.ToString().ToLower()}";
    }
}
