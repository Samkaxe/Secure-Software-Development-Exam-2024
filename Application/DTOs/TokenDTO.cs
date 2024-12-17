namespace Application.DTOs;

public class TokenDTO
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime TokenExpiration { get; set; }
}