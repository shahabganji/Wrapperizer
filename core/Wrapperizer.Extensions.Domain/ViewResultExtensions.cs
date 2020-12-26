using System.Collections.Generic;
using System.Linq;
using Wrapperizer.Domain.Abstractions;

namespace Wrapperizer.Extensions.Domain
{
    public static class ViewResultExtensions
    {
        public static IReadOnlyList<ViewResultMessage> DoneMessages(this IViewResult viewResult)
        {
            return viewResult?.Messages.Where(m => m.Severity == ViewResultMessageSeverity.Done).ToList();
        }

        public static IReadOnlyList<ViewResultMessage> FailedMessages(this IViewResult viewResult)
        {
            return viewResult?.Messages.Where(m => m.Severity == ViewResultMessageSeverity.Fail).ToList();
        }

        public static IReadOnlyList<ViewResultMessage> InfoMessages(this IViewResult viewResult)
        {
            return viewResult?.Messages.Where(m => m.Severity == ViewResultMessageSeverity.Info).ToList();
        }

        public static IReadOnlyList<ViewResultMessage> WarnMessages(this IViewResult viewResult)
        {
            return viewResult?.Messages.Where(m => m.Severity == ViewResultMessageSeverity.Warn).ToList();
        }
    }

}
