using CSharpFunctionalExtensions;

namespace ClassLibrary1;

public interface IDataTree
{
    public string Name { get; }
    Task<Result<IEnumerable<DataEntry>>> GetFiles();
    Task<Result<IEnumerable<DataNode>>> GetDirectories();
}