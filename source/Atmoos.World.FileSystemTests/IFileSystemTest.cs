namespace Atmoos.World.FileSystemTests;

public interface IFileSystemTest
{
    void CreateFile();
    void CreateFileInAntecedentDirs();
    void CreateDirectory();
    void CreateDirectoryInAntecedentDirs();
    void AntecedentDirectoriesAreNotOverwritten();
    void DeleteFileSucceeds();
    void DeleteEmptyDirectorySucceeds();
    void DeleteDirectoryContainingFilesThrows();
    void DeleteDirectoryContainingOtherDirectoriesThrows();
    void DeleteDirectoryRecursivelyRemovesEverything();
    void MoveDirectoryRemovesSourceAndRecreatesTarget();
    void SearchForNonExistentDirectoryFails();
    void SearchForExistingDirectorySucceeds();
}
