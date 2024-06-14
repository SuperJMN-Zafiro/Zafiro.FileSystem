// See https://aka.ms/new-console-template for more information

using Zafiro.FileSystem.Local.Mutable;

Console.WriteLine("Hello World!");
var fs = new System.IO.Abstractions.FileSystem();
var directoryInfo = fs.DirectoryInfo.New("/home/jmn/Escritorio");
var directory = new LocalDynamicDirectory(directoryInfo);
directory.Directories.Subscribe(Console.WriteLine);
Console.ReadLine();