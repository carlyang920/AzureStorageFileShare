Projects:
1. Sample.AzureStorageFileShare: Library Functions
2. Sample.AzureStorageFileShare.Test: Unit Test Project

Note:
1. You can use `Sample.AzureStorageFileShare.Test` to run all functions.
2. You must enter your Storage Account connection string first in testing project.

Functions:
1. `CreateHierarchyFolders(string dirPath)`: Create folder by dirPath hierarchy.
2. `DeleteHierarchyFolders(string dirPath)`: Delete folder by dirPath.
3. `WriteContentAsync(ShareFileClient fileShareClient, Stream content)`: Write content into File.
4. `UploadAsFileAsync(string dirPath, string fileName, byte[] content)`: Upload file with byte[].
5. `UploadAsFileAsync(string dirPath, string fileName, Stream content)`: Upload file with Stream.
6. `FileExistsAsync(string dirPath, string fileName)`: Check if file exists
7. `DeleteFileAsync(string dirPath, string fileName)`: Delete file.
8. `CreateFolderAsync(string dirPath, bool isRecursive = false)`: Create Folder on specific path.
9. `FolderExistsAsync(string dirPath)`: Check if folder exists.
10. `DeleteFolderAsync(string dirPath, bool isRecursive = false)`: Delete specific folder.