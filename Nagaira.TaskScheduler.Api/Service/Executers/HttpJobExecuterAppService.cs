using Hangfire;
using Nagaira.TaskScheduler.Api.Helpers;
using Nagaira.TaskScheduler.Api.Service.Dtos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace Nagaira.TaskScheduler.Api.Service.Executers
{
    public class HttpJobExecuterAppService : IJobExecuter
    {
        private readonly HttpClient _httpClient;
        private readonly GlobalEnvironmentDto _configurationApp;
        private readonly int _timeout;

        public JobExecuterType Type => JobExecuterType.Http;

        JobExecuterType IJobExecuter.Type => JobExecuterType.Http;

        public HttpJobExecuterAppService(HttpClient httpClient, GlobalEnvironmentDto configurationApp)
        {
            _httpClient = httpClient;
            _configurationApp = configurationApp;
            _timeout = int.Parse(_configurationApp.TimeoutInMinutes);
        }

        public async Task DoGetAsync(string baseUrl)
        {
            _httpClient.Timeout = new TimeSpan(0, _timeout, 0);
            await _httpClient.GetAsync(baseUrl);
        }

        public async Task DoPostAsync(string baseUrl, string body)
        {
            _httpClient.Timeout = new TimeSpan(0, _timeout, 0);
            var data = string.IsNullOrEmpty(body) ? null : new StringContent(body, Encoding.UTF8, "application/json");
            await _httpClient.PostAsync(baseUrl, data);
        }

        public async Task DoPutAsync(string baseUrl, string body)
        {
            _httpClient.Timeout = new TimeSpan(0, _timeout, 0);
            var data = string.IsNullOrEmpty(body) ? null : new StringContent(body, Encoding.UTF8, "application/json");
            await _httpClient.PutAsync(baseUrl, data);
        }

        [JobDisplayName("{1}")]
        public async Task ExecuteAsync(JObject jObject, string jobName)
        {
            TaskConfigHttpDto taskConfigHttp = JsonConvert.DeserializeObject<TaskConfigHttpDto>(jObject.ToString());
            if (!string.IsNullOrWhiteSpace(taskConfigHttp.Token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", taskConfigHttp.Token);
            }

            switch (taskConfigHttp.RequestType)
            {
                case "GET":
                    await DoGetAsync(taskConfigHttp.Url);
                    break;
                case "POST":
                    await DoPostAsync(taskConfigHttp.Url, taskConfigHttp.Body);
                    break;
                case "PUT":
                    await DoPutAsync(taskConfigHttp.Url, taskConfigHttp.Body);
                    break;
                default:
                    break;
            }
        }
    }
}
