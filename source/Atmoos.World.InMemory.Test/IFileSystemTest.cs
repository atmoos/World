namespace Atmoos.World.InMemory.Test;

public interface IFileSystemTest
{
    void CreateFileInCurrentDir();
    void CreateFileInAntecedentDirs();
    void CreateDirectoryInCurrentDir();
    void CreateDirectoryInAntecedentDirs();
}
