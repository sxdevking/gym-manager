using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Application.Common.Interfaces;

/// <summary>
/// Servicio de cifrado para datos sensibles
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Cifra un texto usando AES-256
    /// </summary>
    string Encrypt(string plainText, string key);

    /// <summary>
    /// Descifra un texto usando AES-256
    /// </summary>
    string Decrypt(string cipherText, string key);

    /// <summary>
    /// Genera un hash SHA-256
    /// </summary>
    string GenerateHash(string input);

    /// <summary>
    /// Genera una clave aleatoria segura
    /// </summary>
    string GenerateSecureKey(int length = 32);
}
