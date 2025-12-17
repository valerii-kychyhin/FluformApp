namespace FluformApp.Models.Auth
{
    public class LoginResponse
    {
        public string token { get; set; } = string.Empty;
        public string token_type { get; set; } = string.Empty;
    }
}