// See https://aka.ms/new-console-template for more information

using AvaloniaSyncer.Console;
using CSharpFunctionalExtensions;
using Serilog;

Console.WriteLine("Hello, World!");

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var leftPluginResult = await FileSystemPlugin.Create("http://192.168.1.29:8888").Map(plugin => (IFileSystemPlugin)plugin);
var rightPluginResult = await DotnetFsPlugin.Create().Map(plugin => (IFileSystemPlugin)plugin);

var syncer = new Syncer(Maybe<ILogger>.From(Log.Logger));

var op = from leftPlugin in leftPluginResult from rightPlugin in rightPluginResult 
    select syncer.Sync(new FileSource(leftPlugin, "Juegos/ROMs/3DS"), new FileSource(rightPlugin, "D:/OneDrive/Juegos/ROMs/3DS"));

await op.Bind(x => x);