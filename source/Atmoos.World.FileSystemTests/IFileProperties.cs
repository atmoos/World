namespace Atmoos.World.FileSystemTests;

public interface IFileProperties
{
    void SizeOfNonExistingFileIsZero();
    void SizeIncreasesOnFileThatIsWrittenTo();
    void SizeOfNonEmptyFileReturnsActualNumberOfBytes();
    void OpenReadOnNonExistentFileThrows();
    void MultipleReadsOnFileAreAllowed();
    void OpenWriteOnNonExistentFileThrows();
    void OpenWriteOnFileThatIsBeingReadFromThrows();
    void OpenWriteOnFileThatIsBeingWrittenToThrows();
}

