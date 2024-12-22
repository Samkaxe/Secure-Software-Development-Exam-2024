using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Application.Services;

public class EncryptionHelper
{
    private readonly byte[] _key;

    public EncryptionHelper(string secretKey)
    {
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new ArgumentNullException(nameof(secretKey), "Secret key cannot be null or empty.");
        }

        if (secretKey.Length < 32)
        {
            throw new ArgumentException("Secret key must be at least 32 characters (256 bits) long for AES-256.", nameof(secretKey));
        }

        _key = Encoding.UTF8.GetBytes(secretKey.PadRight(32).Substring(0, 32));
    }

    public byte[] EncryptWithMasterKey(byte[] data)
    {
        return EncryptWithSpecificKey(data, _key);
    }

    public byte[] EncryptWithSpecificKey(byte[] data, byte[] key)
    {
        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.GenerateIV();

            using (var msEncrypt = new MemoryStream())
            {
                msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                using (var csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    csEncrypt.Write(data, 0, data.Length);
                }
                return msEncrypt.ToArray();
            }
        }
    }
    
    public byte[] DecryptWithSpecificKey(byte[] cipherText, byte[] key)
    {
        Console.WriteLine("master key value");
        Console.WriteLine(BitConverter.ToString(key));
        Console.WriteLine("ciphertext");
        Console.WriteLine(BitConverter.ToString(cipherText));
        
        Console.WriteLine("Master key length: " + key.Length + " bytes"); //
        using (var aesAlg = Aes.Create())
        {
            Console.WriteLine("Key Length (bytes): " + key.Length);
            aesAlg.Key = key;

            byte[] iv = new byte[aesAlg.BlockSize / 8];
            Array.Copy(cipherText, iv, iv.Length);
            aesAlg.IV = iv;

            using (var msDecrypt = new MemoryStream(cipherText, iv.Length, cipherText.Length - iv.Length))
            using (var csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
            {
                using (var msOutput = new MemoryStream())
                {
                    csDecrypt.CopyTo(msOutput);
                    return msOutput.ToArray();
                }
            }
        }
    }

    public byte[] DecryptWithMasterKey(byte[] cipherText)
    {
        return this.DecryptWithSpecificKey(cipherText, _key);
    }

    public byte[] DeriveKey(string password, string salt, int keySize = 32)
    {
        byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

        return KeyDerivation.Pbkdf2(
            password: password!,
            salt: saltBytes,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: keySize);
    }
}