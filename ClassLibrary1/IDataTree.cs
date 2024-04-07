using CSharpFunctionalExtensions;

namespace ClassLibrary1;

public interface IDataTree
{
    public string Name { get; }
    Task<Result<IEnumerable<IData>>> GetFiles();
    Task<Result<IEnumerable<IDataTree>>> GetDirectories();
}