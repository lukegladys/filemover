using System.IO.Compression;
using Microsoft.Extensions.Logging;

namespace MusicMover;

public interface IFileSystemService
{
    Task MoveAllSongFiles(string sourcePath, string targetPath);
}

public class FileSystemService : IFileSystemService
{
    private readonly ILogger<FileSystemService> _logger;
    
    private List<FileInfo> _songFiles = new();

    public FileSystemService(ILogger<FileSystemService> logger)
    {
        _logger = logger;
    }
    
    public async Task MoveAllSongFiles(string sourcePath, string targetPath)
    {
        _songFiles = new List<FileInfo>();
        
        await WalkDirectoryTree(new DirectoryInfo(sourcePath));
        foreach (var file in _songFiles)
        {
            try
            {
                file.MoveTo(Path.Combine(targetPath, file.Name));
            }
            catch (Exception e)
            {
                _logger.LogWarning(message: e.Message); 
            }
        }
        _logger.LogInformation("Moving complete !");
    }

    private async Task WalkDirectoryTree(DirectoryInfo root)
    {
        await ExtractZipFiles(root);
        await ExtractSongFiles(root);
        await ExtractSubDirectories(root);
    }
    
    private Task<List<FileInfo>> GetFilesByExtensions(DirectoryInfo dir, params string[] extensions)
    {
        if (extensions == null) 
            throw new ArgumentNullException("extensions");
        var files = dir.EnumerateFiles();
        return Task.FromResult(files.Where(f => extensions.Contains(f.Extension)).ToList());
    }
    
    private async Task ExtractZipFiles(DirectoryInfo root)
    {
        try
        {
            var zipFileExtensions = new[] { ".zip"}; 
            var zipFiles = await GetFilesByExtensions(root, zipFileExtensions);
            foreach (var zipFile in zipFiles)
            {
                ZipFile.ExtractToDirectory(zipFile.FullName, root.FullName, overwriteFiles: true);
            }
        }
        // This is thrown if even one of the files requires permissions greater
        // than the application provides.
        catch (UnauthorizedAccessException e)
        {
            // This code just writes out the message and continues to recurse.
            // You may decide to do something different here. For example, you
            // can try to elevate your privileges and access the file again.
            _logger.LogWarning(message: e.Message);
        }
        catch (DirectoryNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }
    }
    
    private async Task ExtractSongFiles(DirectoryInfo root)
    {
        var newSongFiles = new List<FileInfo>();
        
        // First, process all the files directly under this folder
        try
        {
            var songFileExtensions = new[] { ".flac", ".wav", ".wma", ".mp3"};
            newSongFiles = await GetFilesByExtensions(root, songFileExtensions);
            _songFiles.AddRange(newSongFiles);
        }
        // This is thrown if even one of the files requires permissions greater
        // than the application provides.
        catch (UnauthorizedAccessException e)
        {
            // This code just writes out the message and continues to recurse.
            // You may decide to do something different here. For example, you
            // can try to elevate your privileges and access the file again.
            _logger.LogWarning(message: e.Message);
        }
        catch (DirectoryNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }
        
        foreach (var fi in newSongFiles)
        {
            // In this example, we only access the existing FileInfo object. If we
            // want to open, delete or modify the file, then
            // a try-catch block is required here to handle the case
            // where the file has been deleted since the call to TraverseTree().
            Console.WriteLine(fi.FullName);
        }
    }
    
    private async Task ExtractSubDirectories(DirectoryInfo root)
    {
        // Now find all the subdirectories under this directory.
        var newSubDirectories = root.GetDirectories();
        if(newSubDirectories.Length == 0) return;
        
        foreach (var dirInfo in newSubDirectories)
        {
            // Recursive call for each subdirectory.
            await WalkDirectoryTree(dirInfo);
        }
    }
}