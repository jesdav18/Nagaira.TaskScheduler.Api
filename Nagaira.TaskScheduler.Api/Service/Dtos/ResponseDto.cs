namespace Nagaira.TaskScheduler.Api.Service.Dtos
{
    public class ResponseDto
    {
        public string? Id { get; set; }
        public string? Message { get; set; }
        public bool TaskWasCreated { get; set; }
    }
}
