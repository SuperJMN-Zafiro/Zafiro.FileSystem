using System.Reactive.Linq;

namespace Zafiro.FileSystem.VNext.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            var fs = new RegularFileSystem(new System.IO.Abstractions.FileSystem());
            var dir = await fs.GetFile("C:\\Users\\JMN\\Desktop\\AppDir\\AvaloniaSyncer\\Avalonia.Base.dl".Replace("\\", "/"));
        }
    }
}