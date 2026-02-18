namespace OECLWebsite.Infrastructure.Services;

public class FileUploadService : IFileUploadService
{
    private readonly string _uploadRoot;
    private readonly long _maxFileSize;
    private readonly HashSet<string> _allowedExtensions;

    public FileUploadService(IConfiguration configuration, IWebHostEnvironment environment)
    {
        var section = configuration.GetSection("FileUpload");
        _maxFileSize = section.GetValue<long>("MaxFileSizeBytes", 5 * 1024 * 1024);
        _allowedExtensions = section.GetSection("AllowedExtensions")
            .Get<string[]>()?.ToHashSet(StringComparer.OrdinalIgnoreCase)
            ?? [".jpg", ".jpeg", ".png", ".gif", ".webp"];

        var uploadPath = section.GetValue<string>("UploadPath") ?? "wwwroot/uploads";
        _uploadRoot = Path.IsPathRooted(uploadPath)
            ? uploadPath
            : Path.Combine(environment.ContentRootPath, uploadPath);
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, long fileSize, string subfolder)
    {
        if (fileSize > _maxFileSize)
            throw new InvalidOperationException(
                $"File size ({fileSize / 1024.0 / 1024.0:F1} MB) exceeds the maximum allowed size ({_maxFileSize / 1024.0 / 1024.0:F1} MB).");

        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrEmpty(extension) || !_allowedExtensions.Contains(extension))
            throw new InvalidOperationException(
                $"File type '{extension}' is not allowed. Allowed types: {string.Join(", ", _allowedExtensions)}");

        var safeFileName = $"{Guid.NewGuid()}{extension.ToLowerInvariant()}";
        var folderPath = Path.Combine(_uploadRoot, subfolder);
        Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, safeFileName);
        await using var outputStream = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(outputStream);

        return $"/uploads/{subfolder}/{safeFileName}";
    }

    public bool Delete(string relativeUrl)
    {
        if (string.IsNullOrEmpty(relativeUrl)) return false;

        var relativePart = relativeUrl.TrimStart('/');
        var wwwrootPath = Path.GetDirectoryName(_uploadRoot)!;
        var physicalPath = Path.Combine(wwwrootPath, relativePart.Replace('/', Path.DirectorySeparatorChar));

        if (!File.Exists(physicalPath)) return false;

        File.Delete(physicalPath);
        return true;
    }
}
