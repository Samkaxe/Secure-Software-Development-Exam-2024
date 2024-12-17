using System.Security.Cryptography;
using System.Text;

namespace Application.Services;

public static class PasswordHelper
{
    // Generates a cryptographically secure salt
    public static string GenerateSalt()
    {
        var saltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
    }

    // Hashes the password using HMACSHA512, with the salt as the HMAC key
    public static string HashPassword(string password, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        using (var hmac = new HMACSHA512(saltBytes)) // Use salt as HMAC key
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = hmac.ComputeHash(passwordBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }

    // Verifies the entered password against the stored hash and salt
    public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
    {
        var enteredHash = HashPassword(enteredPassword, storedSalt);
        return storedHash == enteredHash;
    }
}