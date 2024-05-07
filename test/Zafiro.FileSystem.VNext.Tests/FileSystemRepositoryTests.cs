using System.IO.Abstractions.TestingHelpers;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Zafiro.FileSystem.Lightweight;
using File = Zafiro.FileSystem.Lightweight.File;

namespace Zafiro.FileSystem.VNext.Tests;

public class FileSystemRepositoryTests
{
    [Fact]
    public async Task Create_new_file()
    {
        var mockFileSystem = new MockFileSystem();
        var fs = new FileSystemRepository(mockFileSystem);
        var file = await fs.AddOrUpdate("c:/Subdir", new File("Test1.txt", "Contenido"));
        file.Should().Succeed();
        mockFileSystem.GetFile("C:\\Subdir\\Test1.txt").TextContents.Should().Be("Contenido");
    }
        
    [Fact]
    public async Task Update_file()
    {
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            ["C:\\Subdir\\Test1.txt"] = new MockFileData("Contenido"),
        });
        var fs = new FileSystemRepository(mockFileSystem);
        var file = await fs.AddOrUpdate("c:/Subdir", new File("Test1.txt", "Nuevo contenido"));
        file.Should().Succeed();
        mockFileSystem.GetFile("C:\\Subdir\\Test1.txt").TextContents.Should().Be("Nuevo contenido");
    }
}