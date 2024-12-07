namespace Core.Entites;

public class Token
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime TokenExpiration { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
    public string DeviceInfo { get; set; }
}