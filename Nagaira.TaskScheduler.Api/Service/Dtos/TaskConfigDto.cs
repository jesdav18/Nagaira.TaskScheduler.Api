using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

namespace Nagaira.TaskScheduler.Api.Service.Dtos
{
    public class TaskConfigDto
    {
        [Required(ErrorMessage = "Debes ingresar el tipo de Job, Http = 1 | SQL = 2"), Range(1, 2)]
        public JobExecuterType JobType { get; set; }
        public string? Cron { get; set; }
        public string? TimeZone { get; set; }
        [Required(ErrorMessage = "El PrefixTask puede ser el nombre de la unidad o departamento.")]
        public string? PrefixTask { get; set; }
        [Required(ErrorMessage = "El nombre de la aplicación es requerido.")]
        public string? AplicationName { get; set; }
        [Required(ErrorMessage = "Debes ingresar un nombre para la aplicación.")]
        public string? JobDescription { get; set; }
        public string? JobName => BuildJobName();
        public DateTime ExecuteDate { get; set; }
        public JObject? JobData { get; set; }

        string BuildJobName()
        {
            return $"{PrefixTask}-{AplicationName}-{JobDescription}".Replace(" ", "_").ToLower();
        }
    }
}
