namespace Atmoos.World.InMemory.Test;

public interface IFileSystemTest
{
    void CreateFile();
    void CreateFileInAntecedentDirs();
    void CreateDirectory();
    void CreateDirectoryInAntecedentDirs();
    void AntecedentDirectoriesAreNotOverwritten();
    void RemoveDirectoryRecursivelyRemovesEverything();
}
