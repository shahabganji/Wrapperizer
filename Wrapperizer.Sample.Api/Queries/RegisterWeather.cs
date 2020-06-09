using System.Threading;
using System.Threading.Tasks;
using Wrapperizer.Abstraction.Cqrs;

namespace Wrapperizer.Sample.Api.Queries
{
    public sealed class RegisterWeather : ICommand<string>
    {
        public string Type { get; set; }
        
        
        internal class RegisterWeatherHandler : ICommandHandler<ICommand<string>,string>
        {
            public async Task<string> Handle(ICommand<string> request, CancellationToken cancellationToken)
            {
                return "";
            }
        }
    }
}
