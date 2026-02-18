namespace OECLWebsite.Infrastructure.Services;

public interface IFileUploadService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, long fileSize, string subfolder);
    bool Delete(string relativeUrl);
}
