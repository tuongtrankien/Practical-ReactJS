namespace SimpleApp.Application.Interfaces;

public interface IBlobStorageService
{
    Task<string> UploadImageAsync(Stream imageStream, string fileName);
}