using System.ComponentModel.DataAnnotations;
using Cocona;
using Microsoft.Extensions.DependencyInjection;
using MusicMover;

var builder = CoconaApp.CreateBuilder();
builder.Services.AddSingleton<IFileSystemService, FileSystemService>();
var app = builder.Build();

app.AddCommand(async (IFileSystemService fileSystemService,
                [Argument(Description = "Folder that contains zip files, folders, and song files")][PathExists]string triageFolder, 
                [Argument(Description = "Target folder to aggregate song files")][PathExists]string targetFolder) =>
{
    await fileSystemService.MoveAllSongFiles(triageFolder, targetFolder);
});

app.Run();

internal class PathExistsAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is string path && Directory.Exists(path))
        {
            return ValidationResult.Success;
        }
        return new ValidationResult($"The path '{value}' is not found.");
    }
}