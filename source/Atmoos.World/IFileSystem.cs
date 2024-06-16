using Atmoos.Sphere.Functional;

namespace Atmoos.World;

public interface IFileSystem :
    IFileAccess,
    IFileCreation,
    IFileDeletion,
    IFileManipulation,
    IDirectoryCreation,
    IDirectoryDeletion,
    IDirectoryManipulation,
    IFileSystemQueries,
    IFileSystemState
{ /* The sum of all file system operations. */ }

public interface IFileSystemState
{
    // ToDo: Should I include the Root on the FileSystem?
    //       Should it be a directory, volume or device?
    static abstract IDirectory CurrentDirectory { get; }
}
public interface IFileSystemQueries
{
    static abstract Result<IFile> Search(FilePath query);
    static abstract Result<IDirectory> Search(Path query);
}
public interface IFileCreation
{
    static abstract IFile Create(FilePath file);
    static abstract IFile Create(IDirectory parent, FileName name);
}
public interface IFileDeletion
{
    static abstract void Delete(IFile file);
}
public interface IFileManipulation
{
    static abstract Task<IFile> Copy(IFile source, NewFile target, CancellationToken token = default);
}

public interface IDirectoryCreation
{
    static abstract IDirectory Create(Path path);
    static abstract IDirectory Create(IDirectory parent, DirectoryName name);
}
public interface IDirectoryDeletion
{
    static abstract void Delete(IDirectory directory, Boolean recursive = false);
}
public interface IDirectoryManipulation
{
    // ToDo: Copy directories
    static abstract IDirectory Move(IDirectory source, in NewDirectory target);
}
