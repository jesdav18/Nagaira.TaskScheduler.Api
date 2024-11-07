namespace Nagaira.TaskScheduler.Client.Dtos
{
    public class ResponseDto
    {
        public string? Id { get; set; }
        public string? Message { get; set; }
        public bool TaskWasCreated { get; set; }
    }
}
