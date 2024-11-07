using System.ComponentModel.DataAnnotations;

namespace Nagaira.TaskScheduler.Api.Service.Dtos
{
    public class TaskConfigHttpDto
    {
        [Required(ErrorMessage = "La uri es requerida.")]
        [Url(ErrorMessage = "No es una url valida.")]
        public string? Url { get; set; }
        [StringRange(AllowableValues = new[] { "GET", "POST", "PUT" })]
        public string? RequestType { get; set; }
        public string? Body { get; set; }
        public string? Token { get; set; } = "";
    }
}
