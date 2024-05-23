using CommandLine;

public class Options
{
    [Option("left", Required = true)]
    public string Left { get; set; }

    [Option("right", Required = true)]
    public string Right { get; set; }
}