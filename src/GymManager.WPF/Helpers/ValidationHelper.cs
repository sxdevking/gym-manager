using System.Text.RegularExpressions;

namespace GymManager.WPF.Helpers;

/// <summary>
/// Helper para validaciones de datos
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// Patron de email valido
    /// </summary>
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Patron de telefono (solo numeros, +, -, parentesis)
    /// </summary>
    private static readonly Regex PhoneRegex = new(
        @"^[\d\+\-\(\)\s]{7,20}$",
        RegexOptions.Compiled);

    /// <summary>
    /// Valida si un email tiene formato correcto
    /// </summary>
    public static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return true; // Email es opcional

        // No permitir espacios
        if (email.Contains(' '))
            return false;

        return EmailRegex.IsMatch(email.Trim());
    }

    /// <summary>
    /// Valida si un telefono tiene formato correcto
    /// </summary>
    public static bool IsValidPhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return true; // Telefono es opcional

        // Limpiar espacios extras
        var cleaned = phone.Trim();
        return PhoneRegex.IsMatch(cleaned);
    }

    /// <summary>
    /// Limpia un texto: trim + eliminar espacios dobles
    /// </summary>
    public static string CleanText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // Trim y reemplazar espacios multiples por uno solo
        return Regex.Replace(text.Trim(), @"\s+", " ");
    }

    /// <summary>
    /// Limpia un email: trim + lowercase + sin espacios
    /// </summary>
    public static string CleanEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return string.Empty;

        return email.Trim().Replace(" ", "").ToLowerInvariant();
    }

    /// <summary>
    /// Limpia un telefono: solo numeros
    /// </summary>
    public static string CleanPhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return string.Empty;

        // Mantener solo numeros
        return Regex.Replace(phone, @"[^\d]", "");
    }

    /// <summary>
    /// Formatea un nombre: Primera letra mayuscula de cada palabra
    /// </summary>
    public static string FormatName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        var cleaned = CleanText(name);
        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cleaned.ToLower());
    }

    /// <summary>
    /// Valida que un string no este vacio
    /// </summary>
    public static bool IsNotEmpty(string? value) =>
        !string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Valida codigo postal mexicano (5 digitos)
    /// </summary>
    public static bool IsValidPostalCode(string? postalCode)
    {
        if (string.IsNullOrWhiteSpace(postalCode))
            return true; // Opcional

        return Regex.IsMatch(postalCode.Trim(), @"^\d{5}$");
    }
}