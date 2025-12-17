public class ErrorResponse
{
    public string message { get; set; }
    public Dictionary<string, List<string>> errors { get; set; }
}