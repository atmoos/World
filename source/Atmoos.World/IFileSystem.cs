using Atmoos.Sphere.Functional;

namespace Atmoos.World;

public interface IFileSystem :
    IFileCreation,
    IFileDeletion,
    IDirectoryCreation,
    IDirectoryDeletion,
    IFileManipulation,
    IDirectoryManipulation,
    IFileSystemQueries,
    IFileSystemState
{ /* The sum of all file system operations. */ }

public interface IFileSystemState
{
    static abstract IDirectory Root { get; }
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
public interface IDirectoryCreation
{
    static abstract IDirectory Create(Path path);
    static abstract IDirectory Create(IDirectory parent, DirectoryName name);
}
public interface IDirectoryDeletion
{
    static abstract void Delete(IDirectory directory, Boolean recursive = false);
}
public interface IFileManipulation
{
    static abstract IFile Move(IFile source, IFile target);
    static abstract IFile Move(IFile source, in NewFile target);
}
public interface IDirectoryManipulation
{
    // ToDo: Copy directories
    static abstract IDirectory Move(IDirectory source, in NewDirectory target);
}
