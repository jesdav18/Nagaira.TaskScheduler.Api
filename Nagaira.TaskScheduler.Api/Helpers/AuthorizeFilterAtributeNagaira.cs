using Hangfire.Dashboard;

namespace Nagaira.TaskScheduler.Api.Helpers
{
    public class AuthorizeFilterAtributeNagaira
    {
        public bool Authorize(DashboardContext context)
        {
            _ = context.GetHttpContext();
            return true;
        }
    }
}
