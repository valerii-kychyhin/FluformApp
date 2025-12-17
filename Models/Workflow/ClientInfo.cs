namespace FluformApp.Models.Workflow;

public class ClientInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime EmailVerifiedAt { get; set; }
    public string SocialLoginType { get; set; } = string.Empty;
    public string SocialLoginId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string DeletedAt { get; set; } = string.Empty;
}