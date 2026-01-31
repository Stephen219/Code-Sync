using FileSync.Domain.Interfaces;

namespace FileSync.Infrastructure.Storage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public LocalFileStorageService(string basePath)
    {
        _basePath = basePath;

        // Ensure base directory exists
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder)
    {
        // Create folder path: basePath/folder/
        var folderPath = Path.Combine(_basePath, folder);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Create unique filename to avoid conflicts
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(folderPath, uniqueFileName);

        // Save file
        using var file = File.Create(filePath);
        await fileStream.CopyToAsync(file);

        // Return relative path for storage in database
        return Path.Combine(folder, uniqueFileName);
    }

    public async Task<Stream?> GetFileAsync(string storagePath)
    {
        var fullPath = Path.Combine(_basePath, storagePath);

        if (!File.Exists(fullPath))
        {
            return null;
        }

        // Return file stream for reading
        return new FileStream(fullPath, FileMode.Open, FileAccess.Read);
    }

    public Task DeleteFileAsync(string storagePath)
    {
        var fullPath = Path.Combine(_basePath, storagePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    public Task DeleteFolderAsync(string folder)
    {
        var folderPath = Path.Combine(_basePath, folder);

        if (Directory.Exists(folderPath))
        {
            Directory.Delete(folderPath, recursive: true);
        }

        return Task.CompletedTask;
    }
}