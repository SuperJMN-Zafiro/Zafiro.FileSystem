﻿namespace Zafiro.FileSystem;

public class Directory : IDirectory
{
    public Directory(string name, IEnumerable<INode> children)
    {
        Name = name;
        Children = children;
    }

    public string Name { get; }
    public IEnumerable<INode> Children { get; }
    public override string ToString() => Name;
}