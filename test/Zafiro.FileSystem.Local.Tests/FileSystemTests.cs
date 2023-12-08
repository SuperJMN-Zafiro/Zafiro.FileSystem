using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Reactive.Linq;
using System.Text;
using FluentAssertions.Extensions;
using Microsoft.Win32.SafeHandles;
using Xunit;

namespace Zafiro.FileSystem.Local.Tests;

public class FileSystemTests
{
    [Fact]
    public async Task Get_file_from_nested_path()
    {
        var fs = new MockLinuxFs();

        var sut = new LinuxZafiroFileSystem(fs);
        var result = await sut.GetFileContents("usr/home/jmn/sample.txt");
    }

    [Fact]
    public async Task Get_root_of_linux_filesystem()
    {
        var fs = new MockLinuxFs();

        var sut = new LinuxZafiroFileSystem(fs);
        var result = await sut.GetDirectoryPaths(new ZafiroPath(string.Empty));
        result.Should().Succeed().And.Subject.Value.Should().BeEquivalentTo(new ZafiroPath[]
        {
            new("usr"),
            new("bin"),
            new("etc"),
        });
    }

    [Fact]
    public async Task Get_root_of_windows_filesystem()
    {
        var mockFileSystem = new MockFileSystem();
        var sut = new WindowsZafiroFileSystem(mockFileSystem);
        var result = await sut.GetDirectoryPaths(new ZafiroPath(string.Empty));
        result.Should().Succeed().And.Subject.Value.Should().BeEquivalentTo(new[] { new ZafiroPath("C:") });
    }

    [Fact]
    public async Task Create_file()
    {
        var mockFileSystem = new MockFileSystem();
        var sut = new WindowsZafiroFileSystem(mockFileSystem);
        var result = await sut.CreateFile("Pepito.txt");
        result.Should().Succeed();
        mockFileSystem.GetFile("Pepito.txt").Contents.Should().BeEmpty();
    }

    [Fact]
    public async Task Get_file()
    {
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            ["Pepito.txt"] = new("Salute")
        });
        var sut = new WindowsZafiroFileSystem(mockFileSystem);
        var result = await sut.GetFileContents("Pepito.txt").ToList();

        Encoding.UTF8.GetString(result.ToArray()).Should().Be("Salute");
    }

    [Fact]
    public async Task Set_file_contents()
    {
        var fs = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            ["Pepito.txt"] = new("Old content")
        });

        var sut = new WindowsZafiroFileSystem(fs);
        IObservable<byte> toWrite = "Salute"u8.ToArray().ToObservable();
        var result = await sut.SetFileContents("Pepito.txt", toWrite);

        result.Should().Succeed();
        fs.GetFile("Pepito.txt").TextContents.Should().Be("Salute");
    }

    [Fact]
    public async Task Create_folder()
    {
        var fs = new MockFileSystem(new Dictionary<string, MockFileData>());

        var sut = new WindowsZafiroFileSystem(fs);
        var result = await sut.CreateDirectory("Folder");

        result.Should().Succeed();
        fs.Directory.Exists("Folder").Should().BeTrue();
    }

    [Fact]
    public async Task Get_file_properties()
    {
        var fs = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            ["File.txt"] = new("Some content") { CreationTime = 30.January(2010)}
        });

        var sut = new WindowsZafiroFileSystem(fs);
        var result = await sut.GetFileProperties("File.txt");

        result.Should().Succeed();
        result.Value.IsHidden.Should().Be(false);
        result.Value.Length.Should().Be(12L);
        result.Value.CreationTime.Should().Be(30.January(2010));
    }
}

public class MockLinuxFs : IFileSystem
{
    public IDirectory Directory { get; } = new LinuxDirectory();
    public IDirectoryInfoFactory DirectoryInfo { get; }
    public IDriveInfoFactory DriveInfo { get; }
    public IFile File => new LinuxFile(this);
    public IFileInfoFactory FileInfo { get; }
    public IFileStreamFactory FileStream { get; }
    public IFileSystemWatcherFactory FileSystemWatcher { get; }
    public IPath Path { get; }
}

public class LinuxFile : IFile
{
    private readonly MockLinuxFs mockLinuxFs;

    public LinuxFile(MockLinuxFs mockLinuxFs)
    {
        this.mockLinuxFs = mockLinuxFs;
    }

    public IFileSystem FileSystem => mockLinuxFs;
    public Task AppendAllLinesAsync(string path, IEnumerable<string> contents, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

    public Task AppendAllLinesAsync(string path, IEnumerable<string> contents, Encoding encoding, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

    public Task AppendAllTextAsync(string path, string? contents, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

    public Task AppendAllTextAsync(string path, string? contents, Encoding encoding, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

    public Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

    public Task<string[]> ReadAllLinesAsync(string path, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

    public Task<string[]> ReadAllLinesAsync(string path, Encoding encoding, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

    public Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

    public Task<string> ReadAllTextAsync(string path, Encoding encoding, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

    public IAsyncEnumerable<string> ReadLinesAsync(string path, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

    public IAsyncEnumerable<string> ReadLinesAsync(string path, Encoding encoding, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

    public Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

    public Task WriteAllLinesAsync(string path, IEnumerable<string> contents, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

    public Task WriteAllLinesAsync(string path, IEnumerable<string> contents, Encoding encoding, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

    public Task WriteAllTextAsync(string path, string? contents, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

    public Task WriteAllTextAsync(string path, string? contents, Encoding encoding, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

    public void AppendAllLines(string path, IEnumerable<string> contents)
    {
        throw new NotImplementedException();
    }

    public void AppendAllLines(string path, IEnumerable<string> contents, Encoding encoding)
    {
        throw new NotImplementedException();
    }

    public void AppendAllText(string path, string? contents)
    {
        throw new NotImplementedException();
    }

    public void AppendAllText(string path, string? contents, Encoding encoding)
    {
        throw new NotImplementedException();
    }

    public StreamWriter AppendText(string path) => throw new NotImplementedException();

    public void Copy(string sourceFileName, string destFileName)
    {
        throw new NotImplementedException();
    }

    public void Copy(string sourceFileName, string destFileName, bool overwrite)
    {
        throw new NotImplementedException();
    }

    public FileSystemStream Create(string path) => throw new NotImplementedException();

    public FileSystemStream Create(string path, int bufferSize) => throw new NotImplementedException();

    public FileSystemStream Create(string path, int bufferSize, FileOptions options) => throw new NotImplementedException();

    public IFileSystemInfo CreateSymbolicLink(string path, string pathToTarget) => throw new NotImplementedException();

    public StreamWriter CreateText(string path) => throw new NotImplementedException();

    public void Decrypt(string path)
    {
        throw new NotImplementedException();
    }

    public void Delete(string path)
    {
        throw new NotImplementedException();
    }

    public void Encrypt(string path)
    {
        throw new NotImplementedException();
    }

    public bool Exists(string? path) => throw new NotImplementedException();

    public FileAttributes GetAttributes(string path) => throw new NotImplementedException();

    public FileAttributes GetAttributes(SafeFileHandle fileHandle) => throw new NotImplementedException();

    public DateTime GetCreationTime(string path) => throw new NotImplementedException();

    public DateTime GetCreationTime(SafeFileHandle fileHandle) => throw new NotImplementedException();

    public DateTime GetCreationTimeUtc(string path) => throw new NotImplementedException();

    public DateTime GetCreationTimeUtc(SafeFileHandle fileHandle) => throw new NotImplementedException();

    public DateTime GetLastAccessTime(string path) => throw new NotImplementedException();

    public DateTime GetLastAccessTime(SafeFileHandle fileHandle) => throw new NotImplementedException();

    public DateTime GetLastAccessTimeUtc(string path) => throw new NotImplementedException();

    public DateTime GetLastAccessTimeUtc(SafeFileHandle fileHandle) => throw new NotImplementedException();

    public DateTime GetLastWriteTime(string path) => throw new NotImplementedException();

    public DateTime GetLastWriteTime(SafeFileHandle fileHandle) => throw new NotImplementedException();

    public DateTime GetLastWriteTimeUtc(string path) => throw new NotImplementedException();

    public DateTime GetLastWriteTimeUtc(SafeFileHandle fileHandle) => throw new NotImplementedException();

    public UnixFileMode GetUnixFileMode(string path) => throw new NotImplementedException();

    public UnixFileMode GetUnixFileMode(SafeFileHandle fileHandle) => throw new NotImplementedException();

    public void Move(string sourceFileName, string destFileName)
    {
        throw new NotImplementedException();
    }

    public void Move(string sourceFileName, string destFileName, bool overwrite)
    {
        throw new NotImplementedException();
    }

    public FileSystemStream Open(string path, FileMode mode) => throw new NotImplementedException();

    public FileSystemStream Open(string path, FileMode mode, FileAccess access) => throw new NotImplementedException();

    public FileSystemStream Open(string path, FileMode mode, FileAccess access, FileShare share) => throw new NotImplementedException();

    public FileSystemStream Open(string path, FileStreamOptions options) => throw new NotImplementedException();

    public FileSystemStream OpenRead(string path)
    {
        return new LinuxSystemStream(new MemoryStream("Hola"u8.ToArray()), path, false);
    }

    public StreamReader OpenText(string path) => throw new NotImplementedException();

    public FileSystemStream OpenWrite(string path) => throw new NotImplementedException();

    public byte[] ReadAllBytes(string path) => throw new NotImplementedException();

    public string[] ReadAllLines(string path) => throw new NotImplementedException();

    public string[] ReadAllLines(string path, Encoding encoding) => throw new NotImplementedException();

    public string ReadAllText(string path) => throw new NotImplementedException();

    public string ReadAllText(string path, Encoding encoding) => throw new NotImplementedException();

    public IEnumerable<string> ReadLines(string path) => throw new NotImplementedException();

    public IEnumerable<string> ReadLines(string path, Encoding encoding) => throw new NotImplementedException();

    public void Replace(string sourceFileName, string destinationFileName, string? destinationBackupFileName)
    {
        throw new NotImplementedException();
    }

    public void Replace(string sourceFileName, string destinationFileName, string? destinationBackupFileName, bool ignoreMetadataErrors)
    {
        throw new NotImplementedException();
    }

    public IFileSystemInfo? ResolveLinkTarget(string linkPath, bool returnFinalTarget) => throw new NotImplementedException();

    public void SetAttributes(string path, FileAttributes fileAttributes)
    {
        throw new NotImplementedException();
    }

    public void SetAttributes(SafeFileHandle fileHandle, FileAttributes fileAttributes)
    {
        throw new NotImplementedException();
    }

    public void SetCreationTime(string path, DateTime creationTime)
    {
        throw new NotImplementedException();
    }

    public void SetCreationTime(SafeFileHandle fileHandle, DateTime creationTime)
    {
        throw new NotImplementedException();
    }

    public void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
    {
        throw new NotImplementedException();
    }

    public void SetCreationTimeUtc(SafeFileHandle fileHandle, DateTime creationTimeUtc)
    {
        throw new NotImplementedException();
    }

    public void SetLastAccessTime(string path, DateTime lastAccessTime)
    {
        throw new NotImplementedException();
    }

    public void SetLastAccessTime(SafeFileHandle fileHandle, DateTime lastAccessTime)
    {
        throw new NotImplementedException();
    }

    public void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
    {
        throw new NotImplementedException();
    }

    public void SetLastAccessTimeUtc(SafeFileHandle fileHandle, DateTime lastAccessTimeUtc)
    {
        throw new NotImplementedException();
    }

    public void SetLastWriteTime(string path, DateTime lastWriteTime)
    {
        throw new NotImplementedException();
    }

    public void SetLastWriteTime(SafeFileHandle fileHandle, DateTime lastWriteTime)
    {
        throw new NotImplementedException();
    }

    public void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
    {
        throw new NotImplementedException();
    }

    public void SetLastWriteTimeUtc(SafeFileHandle fileHandle, DateTime lastWriteTimeUtc)
    {
        throw new NotImplementedException();
    }

    public void SetUnixFileMode(string path, UnixFileMode mode)
    {
        throw new NotImplementedException();
    }

    public void SetUnixFileMode(SafeFileHandle fileHandle, UnixFileMode mode)
    {
        throw new NotImplementedException();
    }

    public void WriteAllBytes(string path, byte[] bytes)
    {
        throw new NotImplementedException();
    }

    public void WriteAllLines(string path, string[] contents)
    {
        throw new NotImplementedException();
    }

    public void WriteAllLines(string path, IEnumerable<string> contents)
    {
        throw new NotImplementedException();
    }

    public void WriteAllLines(string path, string[] contents, Encoding encoding)
    {
        throw new NotImplementedException();
    }

    public void WriteAllLines(string path, IEnumerable<string> contents, Encoding encoding)
    {
        throw new NotImplementedException();
    }

    public void WriteAllText(string path, string? contents)
    {
        throw new NotImplementedException();
    }

    public void WriteAllText(string path, string? contents, Encoding encoding)
    {
        throw new NotImplementedException();
    }
}

public class LinuxSystemStream : FileSystemStream
{
    public LinuxSystemStream(Stream stream, string path, bool isAsync) : base(stream, path, isAsync)
    {
    }
}

public class LinuxDirectory : IDirectory
{
    public IFileSystem FileSystem { get; }
    public IDirectoryInfo CreateDirectory(string path) => throw new NotImplementedException();

    public IDirectoryInfo CreateDirectory(string path, UnixFileMode unixCreateMode) => throw new NotImplementedException();

    public IFileSystemInfo CreateSymbolicLink(string path, string pathToTarget) => throw new NotImplementedException();

    public IDirectoryInfo CreateTempSubdirectory(string? prefix = null) => throw new NotImplementedException();

    public void Delete(string path)
    {
        throw new NotImplementedException();
    }

    public void Delete(string path, bool recursive)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<string> EnumerateDirectories(string path) => throw new NotImplementedException();

    public IEnumerable<string> EnumerateDirectories(string path, string searchPattern) => throw new NotImplementedException();

    public IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption) => throw new NotImplementedException();

    public IEnumerable<string> EnumerateDirectories(string path, string searchPattern, EnumerationOptions enumerationOptions) => throw new NotImplementedException();

    public IEnumerable<string> EnumerateFiles(string path) => throw new NotImplementedException();

    public IEnumerable<string> EnumerateFiles(string path, string searchPattern) => throw new NotImplementedException();

    public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption) => throw new NotImplementedException();

    public IEnumerable<string> EnumerateFiles(string path, string searchPattern, EnumerationOptions enumerationOptions) => throw new NotImplementedException();

    public IEnumerable<string> EnumerateFileSystemEntries(string path) => throw new NotImplementedException();

    public IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern) => throw new NotImplementedException();

    public IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption) => throw new NotImplementedException();

    public IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, EnumerationOptions enumerationOptions) => throw new NotImplementedException();

    public bool Exists(string? path) => throw new NotImplementedException();

    public DateTime GetCreationTime(string path) => throw new NotImplementedException();

    public DateTime GetCreationTimeUtc(string path) => throw new NotImplementedException();

    public string GetCurrentDirectory() => throw new NotImplementedException();

    public string[] GetDirectories(string path) => new []{ "/usr", "/bin", "/etc" };

    public string[] GetDirectories(string path, string searchPattern) => throw new NotImplementedException();

    public string[] GetDirectories(string path, string searchPattern, SearchOption searchOption) => throw new NotImplementedException();

    public string[] GetDirectories(string path, string searchPattern, EnumerationOptions enumerationOptions) => throw new NotImplementedException();

    public string GetDirectoryRoot(string path) => throw new NotImplementedException();

    public string[] GetFiles(string path) => throw new NotImplementedException();

    public string[] GetFiles(string path, string searchPattern) => throw new NotImplementedException();

    public string[] GetFiles(string path, string searchPattern, SearchOption searchOption) => throw new NotImplementedException();

    public string[] GetFiles(string path, string searchPattern, EnumerationOptions enumerationOptions) => throw new NotImplementedException();

    public string[] GetFileSystemEntries(string path) => throw new NotImplementedException();

    public string[] GetFileSystemEntries(string path, string searchPattern) => throw new NotImplementedException();

    public string[] GetFileSystemEntries(string path, string searchPattern, SearchOption searchOption) => throw new NotImplementedException();

    public string[] GetFileSystemEntries(string path, string searchPattern, EnumerationOptions enumerationOptions) => throw new NotImplementedException();

    public DateTime GetLastAccessTime(string path) => throw new NotImplementedException();

    public DateTime GetLastAccessTimeUtc(string path) => throw new NotImplementedException();

    public DateTime GetLastWriteTime(string path) => throw new NotImplementedException();

    public DateTime GetLastWriteTimeUtc(string path) => throw new NotImplementedException();

    public string[] GetLogicalDrives() => throw new NotImplementedException();

    public IDirectoryInfo? GetParent(string path) => throw new NotImplementedException();

    public void Move(string sourceDirName, string destDirName)
    {
        throw new NotImplementedException();
    }

    public IFileSystemInfo? ResolveLinkTarget(string linkPath, bool returnFinalTarget) => throw new NotImplementedException();

    public void SetCreationTime(string path, DateTime creationTime)
    {
        throw new NotImplementedException();
    }

    public void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
    {
        throw new NotImplementedException();
    }

    public void SetCurrentDirectory(string path)
    {
        throw new NotImplementedException();
    }

    public void SetLastAccessTime(string path, DateTime lastAccessTime)
    {
        throw new NotImplementedException();
    }

    public void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
    {
        throw new NotImplementedException();
    }

    public void SetLastWriteTime(string path, DateTime lastWriteTime)
    {
        throw new NotImplementedException();
    }

    public void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
    {
        throw new NotImplementedException();
    }
}