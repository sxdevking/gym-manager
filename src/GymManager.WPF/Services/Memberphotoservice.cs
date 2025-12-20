using System.IO;
using System.Security.Cryptography;

namespace GymManager.WPF.Services;

/// <summary>
/// Servicio para manejo de fotos de miembros
/// Copia las fotos a una carpeta local y gestiona los nombres de archivo
/// </summary>
public static class MemberPhotoService
{
    /// <summary>
    /// Carpeta base para fotos de miembros (relativa al ejecutable)
    /// </summary>
    public static string PhotosBasePath => Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "Assets",
        "MemberPhotos");

    /// <summary>
    /// Asegura que la carpeta de fotos exista
    /// </summary>
    public static void EnsurePhotosFolderExists()
    {
        if (!Directory.Exists(PhotosBasePath))
        {
            Directory.CreateDirectory(PhotosBasePath);
        }
    }

    /// <summary>
    /// Copia una foto al directorio de miembros y retorna la ruta relativa
    /// </summary>
    /// <param name="sourceFilePath">Ruta completa del archivo origen</param>
    /// <param name="memberId">ID del miembro</param>
    /// <returns>Nombre del archivo guardado (ruta relativa)</returns>
    public static string SaveMemberPhoto(string sourceFilePath, Guid memberId)
    {
        EnsurePhotosFolderExists();

        if (!File.Exists(sourceFilePath))
        {
            throw new FileNotFoundException("El archivo de imagen no existe", sourceFilePath);
        }

        // Obtener extension del archivo original
        var extension = Path.GetExtension(sourceFilePath).ToLowerInvariant();

        // Validar que sea una imagen
        var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp" };
        if (!validExtensions.Contains(extension))
        {
            throw new InvalidOperationException($"Formato de imagen no soportado: {extension}");
        }

        // Generar nombre base con el MemberId
        var baseFileName = memberId.ToString();
        var destinationFileName = $"{baseFileName}{extension}";
        var destinationPath = Path.Combine(PhotosBasePath, destinationFileName);

        // Si ya existe un archivo, verificar si es diferente
        if (File.Exists(destinationPath))
        {
            // Comparar si son el mismo archivo (por hash)
            if (AreFilesEqual(sourceFilePath, destinationPath))
            {
                // Es el mismo archivo, no hacer nada
                return destinationFileName;
            }

            // Son diferentes, buscar el siguiente numero disponible
            destinationFileName = GetNextAvailableFileName(baseFileName, extension);
            destinationPath = Path.Combine(PhotosBasePath, destinationFileName);
        }

        // Copiar el archivo
        File.Copy(sourceFilePath, destinationPath, overwrite: true);

        return destinationFileName;
    }

    /// <summary>
    /// Obtiene el siguiente nombre de archivo disponible
    /// Si existe foto.jpg, busca foto-1.jpg, foto-2.jpg, etc.
    /// </summary>
    private static string GetNextAvailableFileName(string baseFileName, string extension)
    {
        int counter = 1;
        string newFileName;
        string newPath;

        do
        {
            newFileName = $"{baseFileName}-{counter}{extension}";
            newPath = Path.Combine(PhotosBasePath, newFileName);
            counter++;
        }
        while (File.Exists(newPath) && counter < 1000);

        return newFileName;
    }

    /// <summary>
    /// Compara dos archivos por su hash MD5 para verificar si son iguales
    /// </summary>
    private static bool AreFilesEqual(string filePath1, string filePath2)
    {
        try
        {
            using var md5 = MD5.Create();

            using var stream1 = File.OpenRead(filePath1);
            var hash1 = md5.ComputeHash(stream1);

            using var stream2 = File.OpenRead(filePath2);
            var hash2 = md5.ComputeHash(stream2);

            return hash1.SequenceEqual(hash2);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Obtiene la ruta completa de una foto a partir de la ruta relativa
    /// </summary>
    /// <param name="relativePhotoPath">Nombre del archivo o ruta relativa</param>
    /// <returns>Ruta completa al archivo</returns>
    public static string GetFullPhotoPath(string? relativePhotoPath)
    {
        if (string.IsNullOrWhiteSpace(relativePhotoPath))
        {
            return string.Empty;
        }

        // Si ya es una ruta absoluta, retornarla
        if (Path.IsPathRooted(relativePhotoPath))
        {
            return relativePhotoPath;
        }

        return Path.Combine(PhotosBasePath, relativePhotoPath);
    }

    /// <summary>
    /// Verifica si existe la foto de un miembro
    /// </summary>
    public static bool PhotoExists(string? relativePhotoPath)
    {
        if (string.IsNullOrWhiteSpace(relativePhotoPath))
        {
            return false;
        }

        var fullPath = GetFullPhotoPath(relativePhotoPath);
        return File.Exists(fullPath);
    }

    /// <summary>
    /// Elimina la foto de un miembro
    /// </summary>
    public static void DeleteMemberPhoto(string? relativePhotoPath)
    {
        if (string.IsNullOrWhiteSpace(relativePhotoPath))
        {
            return;
        }

        var fullPath = GetFullPhotoPath(relativePhotoPath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    /// <summary>
    /// Obtiene todas las fotos de un miembro (incluyendo versiones -1, -2, etc.)
    /// </summary>
    public static IEnumerable<string> GetAllMemberPhotos(Guid memberId)
    {
        EnsurePhotosFolderExists();

        var pattern = $"{memberId}*.*";
        return Directory.GetFiles(PhotosBasePath, pattern)
            .Select(Path.GetFileName)
            .Where(f => f != null)
            .Cast<string>();
    }
}