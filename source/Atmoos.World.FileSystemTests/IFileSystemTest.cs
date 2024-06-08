namespace Atmoos.World.FileSystemTests;

public interface IFileSystemTest
{
    void CreateFile();
    void CreateFileInAntecedentDirs();
    void CreateDirectory();
    void CreateDirectoryInAntecedentDirs();
    void AntecedentDirectoriesAreNotOverwritten();
    void RemoveDirectoryRecursivelyRemovesEverything();
}