﻿namespace Zafiro.FileSystem2;

#pragma warning disable CS1998
public class FileProperties
{
    public bool IsHidden { get; set; }
    public DateTime CreationTime { get; set; }
    public long Length { get; set; }
}