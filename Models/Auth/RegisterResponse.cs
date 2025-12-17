namespace FluformApp.Models.Auth
{
    public class RegisterResponse
    {
        public string token { get; set; } = string.Empty;
        public string token_type { get; set; } = "bearer";
    }
}