namespace Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId, string role);
    string GenerateRefreshToken();
    Guid ValidateAccessToken(string token);
    void RevokeRefreshToken(Guid userId);
}