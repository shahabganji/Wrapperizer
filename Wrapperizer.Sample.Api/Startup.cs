using Microsoft.Extensions.Configuration;

namespace Wrapperizer.Sample.Api
{
    public class Startup : GraceStartup
    {
        public Startup(IConfiguration configuration) : base(configuration)
        {
        }

    }
    
}
