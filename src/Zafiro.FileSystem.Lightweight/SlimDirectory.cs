﻿namespace Zafiro.FileSystem.Lightweight;

public class SlimDirectory : ISlimDirectory
{
    public SlimDirectory(string name, IEnumerable<INode> children)
    {
        Name = name;
        Children = children;
    }

    public string Name { get; }
    public IEnumerable<INode> Children { get; }
}