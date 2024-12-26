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
        var aesGcm = new AesGcm(key, AesGcm.TagByteSizes.MaxSize);

        byte[] nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
        RandomNumberGenerator.Fill(nonce);

        byte[] ciphertext = new byte[data.Length];
        byte[] tag = new byte[AesGcm.TagByteSizes.MaxSize];

        aesGcm.Encrypt(nonce, data, ciphertext, tag);

        byte[] combined = new byte[nonce.Length + ciphertext.Length + tag.Length];
        Array.Copy(nonce, combined, nonce.Length);
        Array.Copy(ciphertext, 0, combined, nonce.Length, ciphertext.Length);
        Array.Copy(tag, 0, combined, nonce.Length + ciphertext.Length, tag.Length);

        return combined;
    }

    public byte[] DecryptWithSpecificKey(byte[] combined, byte[] key)
    {
        var aesGcm = new AesGcm(key, AesGcm.TagByteSizes.MaxSize);

        byte[] nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
        byte[] ciphertext = new byte[combined.Length - AesGcm.NonceByteSizes.MaxSize - AesGcm.TagByteSizes.MaxSize];
        byte[] tag = new byte[AesGcm.TagByteSizes.MaxSize];

        Array.Copy(combined, nonce, nonce.Length);
        Array.Copy(combined, nonce.Length, ciphertext, 0, ciphertext.Length);
        Array.Copy(combined, nonce.Length + ciphertext.Length, tag, 0, tag.Length);

        byte[] plaintext = new byte[ciphertext.Length];

        aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);

        return plaintext;
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
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 100000,
            numBytesRequested: keySize);
    }
}