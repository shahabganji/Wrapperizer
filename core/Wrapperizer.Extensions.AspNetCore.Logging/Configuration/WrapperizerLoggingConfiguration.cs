using System;

namespace Wrapperizer.Extensions.AspNetCore.Logging.Configuration
{
    public sealed class WrapperizerLoggingConfiguration
    {
        public string ApplicationName { get; set; }
        public string SqlConnectionString { get; set; }
        public Uri  ElasticSearchUri { get; set; }

        public string ErrorIndexFormat { get; set; }
        
        public string UsageIndexFormat { get; set; }

        internal WrapperizerLoggingConfiguration()
        {
            this.ElasticSearchUri = new Uri("http://localhost:9200");
            this.ErrorIndexFormat = "error-{0:yyyy.MM.dd}";
            this.UsageIndexFormat = "usage-{0:yyyy.MM.dd}";
        }
        
    }
}
