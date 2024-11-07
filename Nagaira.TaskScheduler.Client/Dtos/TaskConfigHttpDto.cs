namespace Nagaira.TaskScheduler.Client.Dtos
{
    public class TaskConfigHttpDto
    {   
        public string? Url { get; set; }    
        public string? RequestType { get; set; }     
        public string? Body { get; set; }      
        public string? Token { get; set; }
    }
}
