using Newtonsoft.Json.Linq;

namespace Nagaira.TaskScheduler.Api.Service
{
    public interface IJobExecuter
    {
        JobExecuterType Type { get; }
        Task ExecuteAsync(JObject jObject, string jobName = "");
    }
}
