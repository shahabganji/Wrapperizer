using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Wrapperizer.Core.Abstraction
{
    public interface IActionResultAdapter
    {
        public IActionResult Result { get; set; }
    }

    public sealed class ActionResultAdapter : IActionResultAdapter
    {
        public IActionResult Result { get; set; }
    }

}
