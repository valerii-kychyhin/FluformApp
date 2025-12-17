namespace FluformApp.Models.Auth
{
    public class RegisterRequest
    {
        public string name { get; set; } = string.Empty;
        public string surname { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string password_confirmation { get; set; } = string.Empty;
    }
}