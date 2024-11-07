namespace Nagaira.TaskScheduler.Api.Helpers
{
    public class GlobalEnvironmentDto
    {
        public required string TimeZone { get; set; }
        public required string TimeoutInMinutes { get; set; }
    }
}
