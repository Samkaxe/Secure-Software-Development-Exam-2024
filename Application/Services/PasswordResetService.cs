using Application.Interfaces;
using Infrastructure.DataAccessInterfaces;
using Microsoft.Extensions.Configuration;

namespace Application.Services;

public class PasswordResetService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly EncryptionHelper _encryptionHelper;
    private readonly IConfiguration _configuration;

    public PasswordResetService(IUserRepository userRepository, IEmailService emailService, EncryptionHelper encryptionHelper, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _encryptionHelper = encryptionHelper;
        _configuration = configuration;
    }

    public async Task<bool> RequestPasswordResetAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null) return false;

        var resetToken = Guid.NewGuid().ToString();
        user.ResetToken = resetToken;
        user.ResetTokenExpiration = DateTime.UtcNow.AddHours(1);

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        
        var resetLink = $"https://example.com/reset-password?token={resetToken}";
        await _emailService.SendEmailAsync(user.Email, "Password Reset Request", $"Click the link to reset your password: {resetLink}");

        return true;
    }

    public async Task<bool> ResetPasswordAsync(string resetToken, string newPassword)
    {
        var user = await _userRepository.GetByResetTokenAsync(resetToken);
        if (user == null || user.ResetTokenExpiration < DateTime.UtcNow)
            return false;
        
        var salt = PasswordHelper.GenerateSalt();
        var pepper = FetchPasswordPepper();
        var hashedPassword = PasswordHelper.HashPassword(newPassword, salt, pepper);

        user.PasswordSalt = salt;
        user.PasswordHash = hashedPassword;

        var derivedKey = _encryptionHelper.DeriveKey(newPassword, salt);
        var encryptedUeK = _encryptionHelper.EncryptWithMasterKey(derivedKey);
        user.EncryptedUek = encryptedUeK;

        user.ResetToken = null;
        user.ResetTokenExpiration = null;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return true;
    }

    private string FetchPasswordPepper()
    {
        return _configuration.GetSection("PasswordPepper").Value!;
    }
}
