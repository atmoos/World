namespace Atmoos.World;
public interface IFileSystem
{
    // ToDo: Should I include the Root on the FileSystem?
    //       Should it be a directory, volume of device?
    static abstract IDirectoryInfo CurrentDirectory { get; }

    static abstract IFileInfo Create(in NewFile file);
    static abstract IDirectoryInfo Create(in NewDirectory directory);

    // ToDo: Copy should be async...
    static abstract IFileInfo Copy(IFileInfo source, IFileInfo destination);
    static abstract IFileInfo Copy(IFileInfo source, in NewFile destination);

    // ToDo: Copy directories

    static abstract IDirectoryInfo Move(IDirectoryInfo source, in NewDirectory destination);
    static abstract void Delete(IFileInfo file);
    static abstract void Delete(IDirectoryInfo directory, Boolean recursive = false);
}
