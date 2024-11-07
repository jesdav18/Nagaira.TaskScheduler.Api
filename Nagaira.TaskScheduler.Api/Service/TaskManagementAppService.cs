using CronExpressionDescriptor;
using Hangfire;
using Nagaira.TaskScheduler.Api.Service.Dtos;

namespace Nagaira.TaskScheduler.Api.Service
{
    public class TaskManagementAppService
    {
        private readonly List<IJobExecuter> jobExecuters;


        public TaskManagementAppService(IEnumerable<IJobExecuter> jobExecuters)
        {
            this.jobExecuters = jobExecuters.ToList();

        }

        IJobExecuter GetExecuter(JobExecuterType jobExecuterType)
        {
            return jobExecuters.FindLast(e => e.Type == jobExecuterType);
        }

        public ResponseDto AddRecurrentTask(TaskConfigDto taskConfigDto)
        {
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(taskConfigDto.TimeZone);
            ResponseDto response = new ResponseDto()
            {
                Id = taskConfigDto.JobName,
                Message = "La tarea no fue programada.",
                TaskWasCreated = false
            };

            if (string.IsNullOrEmpty(taskConfigDto.Cron))
            {
                response.Message = "Debes enviar le fecha de ejecución de la tarea en tipo cron expression.";
                return response;
            }



            var jobExecuter = GetExecuter(taskConfigDto.JobType);
            RecurringJob.AddOrUpdate(taskConfigDto.JobName, () => jobExecuter.ExecuteAsync(taskConfigDto.JobData, taskConfigDto.JobName), taskConfigDto.Cron, timeZoneInfo);

            string cronDescription = GetCronDescription(taskConfigDto.Cron);
            response.Message = $"La tarea fue programada exitosamente, para ejecutarse con la siguiente frecuencia:  {cronDescription.ToLower()}.";
            response.TaskWasCreated = true;
            return response;
        }

        public ResponseDto AddScheduledTask(TaskConfigDto taskConfigDto)
        {
            string jobId = string.Empty;
            ResponseDto response = new ResponseDto()
            {
                Id = jobId,
                Message = "La tarea no fue programada.",
                TaskWasCreated = false
            };

            var jobExecuter = GetExecuter(taskConfigDto.JobType);
            jobId = BackgroundJob.Schedule(() => jobExecuter.ExecuteAsync(taskConfigDto.JobData, taskConfigDto.JobName), taskConfigDto.ExecuteDate);

            if (string.IsNullOrEmpty(jobId))
            {
                return response;
            }

            response.Id = jobId;
            response.Message = $"La tarea fue programada exitosamente, para ejecutarse el {taskConfigDto.ExecuteDate.ToLongDateString()} a las {taskConfigDto.ExecuteDate.ToLongTimeString()}";
            response.TaskWasCreated = true;
            return response;
        }


        public ResponseDto AddEnqueueTask(TaskConfigDto taskConfigDto)
        {
            string jobId = string.Empty;
            ResponseDto response = new ResponseDto()
            {
                Id = jobId,
                Message = "La tarea no fue programada."
            };

            var jobExecuter = GetExecuter(taskConfigDto.JobType);

            jobId = BackgroundJob.Enqueue(() => jobExecuter.ExecuteAsync(taskConfigDto.JobData, taskConfigDto.JobName));

            if (string.IsNullOrEmpty(jobId))
            {
                return response;
            }

            response.Id = jobId;
            response.Message = $"La tarea fue programada exitosamente.";
            return response;
        }

        public ResponseDto DeletedRecurrentTask(string jobId)
        {
            ResponseDto response = new ResponseDto()
            {
                Id = jobId,
                Message = "La tarea no fue eliminada.",
                TaskWasCreated = false
            };
            try
            {
                RecurringJob.RemoveIfExists(jobId);
            }
            catch (Exception)
            {
                return response;
            }

            response.Id = jobId;
            response.Message = "La tarea fue eliminada correctamente.";
            response.TaskWasCreated = true;
            return response;
        }

        public ResponseDto DeleteScheduledTask(string jobId)
        {
            ResponseDto response = new ResponseDto()
            {
                Id = jobId,
                Message = "La tarea no fue eliminada.",
                TaskWasCreated = false
            };
            try
            {
                if (BackgroundJob.Delete(jobId))
                {

                    response.Id = jobId;
                    response.Message = "La tarea fue eliminada correctamente.";
                    response.TaskWasCreated = true;
                    return response;
                };
            }
            catch (Exception)
            {
                return response;
                throw;
            }


            return response;
        }

        protected virtual string GetCronDescription(string cron)
        {
            return ExpressionDescriptor.GetDescription(cron, new Options()
            {
                DayOfWeekStartIndexZero = false,
                Use24HourTimeFormat = true,
                Locale = "es"
            });
        }
    }
}
