using System.Security.Cryptography;
using System.Text;

namespace Application.Services;

public static class PasswordHelper
{
   
    public static string GenerateSalt()
    {
        var saltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
    }

    
    public static string HashPassword(string password, string salt)
    {
        using (var hmac = new HMACSHA512())
        {
            var saltedPassword = Encoding.UTF8.GetBytes(password + salt);
            var hashBytes = hmac.ComputeHash(saltedPassword);
            return Convert.ToBase64String(hashBytes);
        }
    }

   
    public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
    {
        var enteredHash = HashPassword(enteredPassword, storedSalt);
        return storedHash == enteredHash;
    }
}