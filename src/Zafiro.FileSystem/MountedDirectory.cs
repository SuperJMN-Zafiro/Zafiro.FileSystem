namespace Zafiro.FileSystem;

public class MountedDirectory : DirectoryWithPathTranslation
{
    public MountedDirectory(IZafiroDirectory directory, IZafiroDirectory root) : base(directory, root)
    {
    }

    protected override ZafiroPath TranslatePath(ZafiroPath path) => Root.Path.Combine(path);

    protected override IZafiroFile CreateFile(IZafiroFile file) => new TranslatedFile(file, Root, TranslatePath);

    protected override IZafiroDirectory CreateDirectory(IZafiroDirectory directory) => throw new NotImplementedException();
}