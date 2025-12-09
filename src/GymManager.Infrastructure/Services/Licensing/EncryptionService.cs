using System.Security.Cryptography;
using System.Text;
using GymManager.Application.Common.Interfaces;

namespace GymManager.Infrastructure.Services.Licensing;

/// <summary>
/// Implementación del servicio de cifrado AES-256
/// </summary>
public class EncryptionService : IEncryptionService
{
    private const int KeySize = 256;
    private const int BlockSize = 128;
    private const int Iterations = 10000;

    public string Encrypt(string plainText, string key)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentNullException(nameof(plainText));
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        byte[] salt = GenerateRandomBytes(16);
        byte[] iv = GenerateRandomBytes(16);

        using var deriveBytes = new Rfc2898DeriveBytes(key, salt, Iterations, HashAlgorithmName.SHA256);
        byte[] keyBytes = deriveBytes.GetBytes(KeySize / 8);

        using var aes = Aes.Create();
        aes.KeySize = KeySize;
        aes.BlockSize = BlockSize;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = keyBytes;
        aes.IV = iv;

        using var encryptor = aes.CreateEncryptor();
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        // Combinar: salt + iv + encrypted
        byte[] result = new byte[salt.Length + iv.Length + encryptedBytes.Length];
        Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
        Buffer.BlockCopy(iv, 0, result, salt.Length, iv.Length);
        Buffer.BlockCopy(encryptedBytes, 0, result, salt.Length + iv.Length, encryptedBytes.Length);

        return Convert.ToBase64String(result);
    }

    public string Decrypt(string cipherText, string key)
    {
        if (string.IsNullOrEmpty(cipherText))
            throw new ArgumentNullException(nameof(cipherText));
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        byte[] allBytes = Convert.FromBase64String(cipherText);

        // Extraer: salt + iv + encrypted
        byte[] salt = new byte[16];
        byte[] iv = new byte[16];
        byte[] encryptedBytes = new byte[allBytes.Length - 32];

        Buffer.BlockCopy(allBytes, 0, salt, 0, 16);
        Buffer.BlockCopy(allBytes, 16, iv, 0, 16);
        Buffer.BlockCopy(allBytes, 32, encryptedBytes, 0, encryptedBytes.Length);

        using var deriveBytes = new Rfc2898DeriveBytes(key, salt, Iterations, HashAlgorithmName.SHA256);
        byte[] keyBytes = deriveBytes.GetBytes(KeySize / 8);

        using var aes = Aes.Create();
        aes.KeySize = KeySize;
        aes.BlockSize = BlockSize;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = keyBytes;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

        return Encoding.UTF8.GetString(decryptedBytes);
    }

    public string GenerateHash(string input)
    {
        if (string.IsNullOrEmpty(input))
            throw new ArgumentNullException(nameof(input));

        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = SHA256.HashData(inputBytes);

        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    public string GenerateSecureKey(int length = 32)
    {
        byte[] randomBytes = GenerateRandomBytes(length);
        return Convert.ToBase64String(randomBytes);
    }

    private static byte[] GenerateRandomBytes(int length)
    {
        byte[] bytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return bytes;
    }
}