namespace Atmoos.World;

public interface IFileSystem :
    IFileAccess,
    IFileCreation,
    IFileDeletion,
    IFileManipulation,
    IDirectoryCreation,
    IDirectoryDeletion,
    IDirectoryManipulation,
    IFileSystemState
{ /* The sum of all file system operations. */ }

public interface IFileSystemState
{
    // ToDo: Should I include the Root on the FileSystem?
    //       Should it be a directory, volume or device?
    static abstract IDirectoryInfo CurrentDirectory { get; }
}
public interface IFileCreation
{
    static abstract IFileInfo Create(in NewFile file);
}
public interface IFileDeletion
{
    static abstract void Delete(IFileInfo file);
}
public interface IFileManipulation
{
    static abstract Task<IFileInfo> Copy(IFileInfo source, NewFile destination, CancellationToken token = default);
    static abstract Task<IFileInfo> Copy(IFileInfo source, IFileInfo destination, CancellationToken token = default);
}

public interface IDirectoryCreation
{
    static abstract IDirectoryInfo Create(in NewDirectory directory);
}
public interface IDirectoryDeletion
{
    static abstract void Delete(IDirectoryInfo directory, Boolean recursive = false);
}
public interface IDirectoryManipulation
{
    // ToDo: Copy directories
    static abstract IDirectoryInfo Move(IDirectoryInfo source, in NewDirectory destination);
}
