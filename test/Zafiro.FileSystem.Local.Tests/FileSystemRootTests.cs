﻿using System.IO.Abstractions.TestingHelpers;
using CSharpFunctionalExtensions;
using Xunit;

namespace Zafiro.FileSystem.Local.Tests;

public class FileSystemRootTests
{
    [Fact]
    public async Task Tested()
    {
        var mfs = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            ["Test.txt"] = new("asdf"),
        });
        var sut = new FileSystemRoot(new ObservableFileSystem(new LocalZafiroFileSystem(mfs)));
        var dir = sut.GetDirectory("NewDir");

        await dir.Exists.TapIf(b => !b, () => dir.Create());

        mfs.DirectoryInfo.New("NewDir").Exists.Should().BeTrue();
    }
}