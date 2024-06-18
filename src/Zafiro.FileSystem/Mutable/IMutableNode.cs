﻿using Zafiro.FileSystem.Core;

namespace Zafiro.FileSystem.Mutable.Mutable;

public interface IMutableNode : INode
{
    public bool IsHidden { get; }
}