namespace Atmoos.World.FileSystemTests;

public interface IFileSystemScenarios
{
    void CreateFileSucceeds();
    void CreateFileInAntecedentDirs();
    void CreateDirectorySucceeds();
    void CreateDirectoryInAntecedentDirs();
    void AntecedentDirectoriesAreNotOverwritten();
    void DeleteFileSucceeds();
    void DeleteEmptyDirectorySucceeds();
    void DeleteDirectoryContainingFilesThrows();
    void DeleteDirectoryContainingOtherDirectoriesThrows();
    void DeleteDirectoryRecursivelyRemovesEverything();
    void SearchForNonExistentFileFails();
    void SearchForExistingFileSucceeds();
    void SearchForNonExistentDirectoryFails();
    void SearchForExistingDirectorySucceeds();
    void MoveToNewFileFailsWhenTargetAlreadyExists();
    void MoveToNewFileMovesContentAndRemovesSource();
    void MoveExistingFileOverwritesContentAndRemovesSource();
    void MoveDirectoryRemovesSourceAndRecreatesTarget();
}
