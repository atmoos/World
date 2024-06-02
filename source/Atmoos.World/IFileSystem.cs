namespace Atmoos.World;
public interface IFileSystem
{
    static abstract IDirectoryInfo CurrentDirectory { get; }
    static abstract IFileInfo Create(in NewFile file);
    static abstract IDirectoryInfo Create(in NewDirectory directory);
    static abstract IFileInfo Copy(IFileInfo source, IFileInfo destination);
    static abstract IFileInfo Copy(IFileInfo source, in NewFile destination);
    static abstract IDirectoryInfo Move(IDirectoryInfo source, in NewDirectory destination);
    static abstract void Delete(IFileInfo file);
    static abstract void Delete(IDirectoryInfo directory, Boolean recursive = false);
}
