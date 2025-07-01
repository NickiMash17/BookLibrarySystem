using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace BookLibrarySystem.Filters
{
    public class ActionLoggingFilter : IActionFilter
    {
        private readonly ILogger<ActionLoggingFilter> _logger;

        public ActionLoggingFilter(ILogger<ActionLoggingFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation($"Action {context.ActionDescriptor.DisplayName} starting.");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation($"Action {context.ActionDescriptor.DisplayName} finished.");
        }
    }
} 