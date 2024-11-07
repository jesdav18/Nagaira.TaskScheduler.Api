using Microsoft.AspNetCore.Mvc;
using Nagaira.TaskScheduler.Api.Helpers;
using Nagaira.TaskScheduler.Api.Service;
using Nagaira.TaskScheduler.Api.Service.Dtos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

namespace Nagaira.TaskScheduler.Api.Controllers
{
    [Route("tasks-managements")]
    public class TaskManagementController : ControllerBase
    {
        private readonly TaskManagementAppService _taskManagementAppService;
        private readonly ConfigurationApp _configurationApp;

        public TaskManagementController(TaskManagementAppService taskManagementAppService, ConfigurationApp configurationApp)
        {
            _taskManagementAppService = taskManagementAppService;
            _configurationApp = configurationApp;
        }

        [HttpPost("recurring-tasks")]
        public IActionResult AddTaskRecurrent([FromBody] TaskConfigDto taskConfigDto)
        {
            taskConfigDto.TimeZone = _configurationApp.TimeZone;

            if (taskConfigDto.JobType == JobExecuterType.Http)
            {
                string modelState = ValidateState(taskConfigDto.JobData);
                if (!string.IsNullOrEmpty(modelState)) return BadRequest(modelState);
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Ok(_taskManagementAppService.AddRecurrentTask(taskConfigDto));
        }

        [HttpPost("enqueue-tasks")]
        public IActionResult AddEnqueueTask([FromBody] TaskConfigDto taskConfigDto)
        {
            taskConfigDto.TimeZone = _configurationApp.TimeZone;
            if (taskConfigDto.JobType == JobExecuterType.Http)
            {
                string modelState = ValidateState(taskConfigDto.JobData);
                if (!string.IsNullOrEmpty(modelState)) return BadRequest(modelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(_taskManagementAppService.AddEnqueueTask(taskConfigDto));
        }


        [HttpDelete("recurring-tasks")]
        public IActionResult DeletedRecurrentTask([FromQuery] string jobId)
        {
            if (string.IsNullOrEmpty(jobId))
            {
                return BadRequest("Debes enviar el JobId");
            }

            return Ok(_taskManagementAppService.DeletedRecurrentTask(jobId));
        }

        [HttpPost("scheduled-tasks")]
        public IActionResult AddScheduledTask([FromBody] TaskConfigDto taskConfigDto)
        {
            taskConfigDto.TimeZone = _configurationApp.TimeZone;
            if (taskConfigDto.JobType == JobExecuterType.Http)
            {
                string modelState = ValidateState(taskConfigDto.JobData);
                if (!string.IsNullOrEmpty(modelState)) return BadRequest(modelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(_taskManagementAppService.AddScheduledTask(taskConfigDto));
        }

        [HttpDelete("scheduled-tasks")]
        public IActionResult DeleteScheduledTask([FromQuery] string jobId)
        {
            if (string.IsNullOrEmpty(jobId))
            {
                return BadRequest("Debes enviar el JobId");
            }
            return Ok(_taskManagementAppService.DeleteScheduledTask(jobId));
        }

        private string ValidateState(JObject jObject)
        {
            TaskConfigHttpDto taskConfigHttp = new TaskConfigHttpDto();
            taskConfigHttp = JsonConvert.DeserializeObject<TaskConfigHttpDto>(jObject.ToString());
            var results = new List<ValidationResult>();
            var context = new ValidationContext(taskConfigHttp, null, null);

            if (!Validator.TryValidateObject(taskConfigHttp, context, results))
            {
                foreach (var error in results)
                {
                    return error.ErrorMessage;
                }
            }
            return string.Empty;
        }
    }
}
