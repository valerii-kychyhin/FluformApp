namespace FluformApp.Models.Profile
{
    public class ClientResourceResponse
    {
        public string id { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string surname { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string? email_verified_at { get; set; }
        public string? social_login_type { get; set; }
        public string? social_login_id { get; set; }
        public string? created_at { get; set; }
        public string? updated_at { get; set; }
        public string? deleted_at { get; set; }
    }
}