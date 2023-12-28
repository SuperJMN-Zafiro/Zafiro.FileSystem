using System.IO.Abstractions.TestingHelpers;
using System.Reactive.Linq;
using System.Text;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Xunit;
using Zafiro.FileSystem.Actions;
using Zafiro.FileSystem.Local;

namespace Zafiro.FileSystem.Tests.Actions;

public class CopyFileActionTests
{
    [Fact]
    public async Task Should_copy_contents()
    {
        var fs = GetFileSystem(new Dictionary<string, MockFileData>()
        {
             ["one.txt"] = new MockFileData("Saludos"),
             ["two.txt"] = new MockFileData("Cordiales"),
        });

        var result = await CopyFileAction.Create(fs.GetFile("one.txt"), fs.GetFile("two.txt")).Bind(c => c.Execute());
        result.Should().Succeed();
        var data = fs.GetFile("two.txt").Contents.ToEnumerable().ToArray();
        var stri = Encoding.UTF8.GetString(data);
        stri.Should().Be("Saludos");
    }

    private IFileSystemRoot GetFileSystem(IDictionary<string, MockFileData> files)
    {
        return new FileSystemRoot(new ObservableFileSystem(new WindowsZafiroFileSystem(new MockFileSystem(files))));
    }
}