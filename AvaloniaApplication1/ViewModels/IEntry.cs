using Zafiro.FileSystem;

namespace AvaloniaApplication1.ViewModels;

public interface IEntry : INamed
{
    public string Key { get;  }
}