using Newtonsoft.Json.Linq;
using System;

namespace Nagaira.TaskScheduler.Client.Dtos
{
    public class TaskConfigDto
    {
        public JobExecuterType? JobType { get; set; }  
        public string? Cron { get; set; }
        public string? PrefixTask { get; set; }
        public string? AplicationName { get; set; }
        public string? JobDescription { get; set; }
        public DateTime ExecuteDate { get; set; }
        public JObject? JobData { get; set; }
    }
}
