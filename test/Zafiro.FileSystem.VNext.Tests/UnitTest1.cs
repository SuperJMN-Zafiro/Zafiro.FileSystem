using System.Reactive.Linq;
using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem.VNext.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            var fs = new FileSystemRepository(new System.IO.Abstractions.FileSystem());
            var file = await fs.GetFile("C:\\Users\\JMN\\Desktop\\AppDir\\AvaloniaSyncer\\Avalonia.Base.dl".Replace("\\", "/"));
            var dir = await fs.GetDirectory("C:\\Users\\JMN\\Desktop\\AppDir\\AvaloniaSyncer\\Avalonia.Base.dl".Replace("\\", "/"));
            file.Tap(f => f.Execute(g => { }));
        }
    }
}