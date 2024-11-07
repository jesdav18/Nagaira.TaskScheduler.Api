using Nagaira.TaskScheduler.Client.Dtos;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nagaira.TaskScheduler.Client
{
    public class TaskSchedulerService
    {
        private HttpClient _httpClient;
        protected readonly string baseUrl;
        protected readonly string prefix;
        void CreateHttpClient()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.Timeout = new TimeSpan(0, 10, 0);
        }

        public TaskSchedulerService(string baseUrl)
        {
            this.baseUrl = baseUrl;
            prefix = "tasks-managements";
        }

        private static StringContent ToContent(TaskConfigDto config)
        {
            var json = JsonConvert.SerializeObject(config);
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");

            return data;
        }
        public async Task<ResponseDto> AddTaskRecurrent(TaskConfigDto config)
        {
            CreateHttpClient();
            ResponseDto responseData = CrearResponseDto();
            StringContent data = ToContent(config);

            HttpResponseMessage httpResponse;

            try
            {
                httpResponse = await _httpClient.PostAsync($"{prefix}/recurring-tasks", data);
            }
            catch (Exception ex)
            {
                responseData.Message = ex.Message.ToString();
                return responseData;
            }

            var responseMenssage = await httpResponse.Content.ReadAsStringAsync();
            responseData = JsonConvert.DeserializeObject<ResponseDto>(responseMenssage);

            if (!httpResponse.IsSuccessStatusCode)
            {
                return responseData;
            }

            return responseData;
        }

        public async Task<ResponseDto> AddScheduledTask(TaskConfigDto config)
        {
            CreateHttpClient();
            ResponseDto responseData = CrearResponseDto();
            StringContent data = ToContent(config);

            HttpResponseMessage httpResponse = null;
            try
            {
                httpResponse = await _httpClient.PostAsync($"{prefix}/scheduled-tasks", data);
            }
            catch (Exception ex)
            {
                responseData.Message = ex.Message.ToString();
                return responseData;
            }
            var responseMenssage = await httpResponse.Content.ReadAsStringAsync();
            responseData = JsonConvert.DeserializeObject<ResponseDto>(responseMenssage);

            if (!httpResponse.IsSuccessStatusCode)
            {
                return responseData;
            }

            return responseData;
        }

        public async Task<ResponseDto> AddEnqueueTask(TaskConfigDto config)
        {
            CreateHttpClient();

            ResponseDto responseData = CrearResponseDto();
            StringContent data = ToContent(config);

            HttpResponseMessage httpResponse = null;
            try
            {
                httpResponse = await _httpClient.PostAsync($"{prefix}/enqueue-tasks", data);
            }
            catch (Exception ex)
            {
                responseData.Message = ex.Message.ToString();
                return responseData;
            }
            var responseMenssage = await httpResponse.Content.ReadAsStringAsync();
            responseData = JsonConvert.DeserializeObject<ResponseDto>(responseMenssage);

            if (!httpResponse.IsSuccessStatusCode)
            {
                return responseData;
            }

            return responseData;
        }

        public async Task<ResponseDto> DeletedRecurrentTask(string jobId)
        {
            CreateHttpClient();
            ResponseDto responseData = CrearResponseDto();
            HttpResponseMessage httpResponse = null;
            try
            {
                httpResponse = await _httpClient.DeleteAsync($"{prefix}/recurring-tasks?jobId={jobId}");
            }
            catch (Exception ex)
            {
                responseData.Message = ex.Message.ToString();
                return responseData;
            }
            var responseMenssage = await httpResponse.Content.ReadAsStringAsync();
            responseData = JsonConvert.DeserializeObject<ResponseDto>(responseMenssage);
            if (!httpResponse.IsSuccessStatusCode)
            {
                return responseData;
            }

            return responseData;
        }

        public async Task<ResponseDto> DeleteScheduledTask(string jobId)
        {
            CreateHttpClient();
            ResponseDto responseData = CrearResponseDto();
            HttpResponseMessage httpResponse = null;
            try
            {
                httpResponse = await _httpClient.DeleteAsync($"{prefix}/scheduled-tasks?jobId={jobId}");
            }
            catch (Exception ex)
            {
                responseData.Message = ex.Message.ToString();
                return responseData;
            }

            var responseMenssage = await httpResponse.Content.ReadAsStringAsync();
            responseData = JsonConvert.DeserializeObject<ResponseDto>(responseMenssage);
            if (!httpResponse.IsSuccessStatusCode)
            {
                return responseData;
            }

            return responseData;
        }

        private ResponseDto CrearResponseDto()
        {
            return new ResponseDto()
            {
                Id = "",
                Message = "La tarea no fue programada.",
            };
        }
    }
}
