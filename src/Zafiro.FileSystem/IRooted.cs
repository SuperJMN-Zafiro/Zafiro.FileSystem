﻿namespace Zafiro.FileSystem;

public interface IRooted
{
    ZafiroPath Path { get; }
}

public interface IRooted<out T> : IRooted
{
    T Value { get; }
}