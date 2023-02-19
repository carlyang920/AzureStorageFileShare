using System.Text;
using Azure;
using Azure.Storage.Files.Shares;

namespace Sample.AzureStorageFileShare.FileShare
{
    /// <summary>
    /// The library for accessing Azure storage file share.
    ///
    /// Note:
    /// Please make sure to initialize one instance for one file share.
    /// </summary>
    public class FileShareService
    {
        /// <summary>
        /// The file share client to handle file share operations.
        /// </summary>
        private readonly ShareClient _shareClient;

        /// <summary>
        /// Use this to create file share client with each share name.
        ///
        /// Note:
        /// Please make sure one share client instance to handle one share name.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="shareName"></param>
        public FileShareService(
            string connectionString,
            string shareName
        )
        {
            _shareClient = GetShareClient(connectionString, shareName);
        }

        #region private functions

        /// <summary>
        /// Generate file share client instance.
        /// </summary>
        /// <param name="connectionString">Azure Storage connection string</param>
        /// <param name="shareName">File share name</param>
        /// <returns></returns>
        private static ShareClient GetShareClient(
            string connectionString,
            string shareName
        )
        {
            var shareClient = new ShareClient(connectionString, shareName.ToLower());

            shareClient.CreateIfNotExists();

            return shareClient;
        }

        /// <summary>
        /// Create hierarchy folders
        /// </summary>
        /// <param name="dirPath"></param>
        private void CreateHierarchyFolders(string dirPath)
        {
            void CreateFolder(string p)
            {
                var dirClient = _shareClient.GetDirectoryClient(p);
                dirClient.CreateIfNotExistsAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            }

            var stackPath = string.Empty;

            dirPath.Split('/').Where(d => !string.IsNullOrEmpty(d)).ToList().ForEach(d =>
            {
                if (string.IsNullOrEmpty(d)) return;

                stackPath += $"/{d}";

                CreateFolder(stackPath);
            });
        }

        /// <summary>
        /// Delete hierarchy folders
        /// </summary>
        /// <param name="dirPath"></param>
        private void DeleteHierarchyFolders(string dirPath)
        {
            void DeleteFolder(string p)
            {
                var dirClient = _shareClient.GetDirectoryClient(p);
                dirClient.DeleteIfExistsAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            }

            var stackPath = dirPath;
            var splitList = dirPath.Split('/').Where(d => !string.IsNullOrEmpty(d)).ToList();

            for (var i = splitList.Count - 1; i >= 0; i--)
            {
                if (!string.IsNullOrEmpty(stackPath))
                    DeleteFolder(stackPath);

                stackPath = stackPath?.Replace($"{splitList[i]}", string.Empty).TrimEnd('/');
            }
        }

        /// <summary>
        /// Write content to file share in batches.
        /// </summary>
        /// <param name="fileShareClient"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private async Task WriteContentAsync(ShareFileClient fileShareClient, Stream content)
        {
            const int blockSize = 4000000;

            //http range offset
            long offset = 0;

            await fileShareClient.CreateAsync(content.Length);

            using var reader = new BinaryReader(content);

            //Write in batches
            while (true)
            {
                var buffer = reader.ReadBytes(blockSize);
                if (buffer.Length == 0)
                    break;

                var uploadChunk = new MemoryStream();
                uploadChunk.Write(buffer, 0, buffer.Length);
                uploadChunk.Position = 0;

                var httpRange = new HttpRange(offset, buffer.Length);
                await fileShareClient.UploadRangeAsync(httpRange, uploadChunk);

                //Shift the offset by number of bytes already written
                offset += buffer.Length;
            }
        }

        #endregion

        #region public functions

        /// <summary>
        /// Save content with bytes into file share folder
        /// </summary>
        /// <param name="dirPath">Directory path</param>
        /// <param name="fileName">File name</param>
        /// <param name="content">Data bytes</param>
        public async Task UploadAsFileAsync(string dirPath, string fileName, byte[] content)
        {
            dirPath = dirPath.Replace("\\", "/");

            CreateHierarchyFolders(dirPath);

            var dirClient = _shareClient.GetDirectoryClient(dirPath);
            
            using var ms = new MemoryStream(content);

            // Get a reference to a file and upload it
            await WriteContentAsync(dirClient.GetFileClient(fileName), ms);
        }

        /// <summary>
        /// Save content with Stream into file share folder
        /// </summary>
        /// <param name="dirPath">Directory path</param>
        /// <param name="fileName">File name</param>
        /// <param name="content">Data stream</param>
        public async Task UploadAsFileAsync(string dirPath, string fileName, Stream content)
        {
            dirPath = dirPath.Replace("\\", "/");

            CreateHierarchyFolders(dirPath);

            var dirClient = _shareClient.GetDirectoryClient(dirPath);

            var data = await new StreamReader(content).ReadToEndAsync();

            if (content.CanSeek) content.Seek(0, SeekOrigin.Begin);
            
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(data));

            await WriteContentAsync(dirClient.GetFileClient(fileName), ms);
        }

        /// <summary>
        /// Check file exists or not
        /// </summary>
        /// <param name="dirPath">Directory path</param>
        /// <param name="fileName">File name</param>
        public async Task<bool> FileExistsAsync(string dirPath, string fileName)
        {
            dirPath = dirPath.Replace("\\", "/");

            var result = false;

            var dirClient = _shareClient.GetDirectoryClient(dirPath);

            if (!await dirClient.ExistsAsync()) return false;

            // Get a reference to a file and upload it
            var fileShareClient = dirClient.GetFileClient(fileName);

            if (await fileShareClient.ExistsAsync())
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="dirPath">Directory path</param>
        /// <param name="fileName">File name</param>
        public async Task<bool> DeleteFileAsync(string dirPath, string fileName)
        {
            dirPath = dirPath.Replace("\\", "/");

            var result = false;

            var dirClient = _shareClient.GetDirectoryClient(dirPath);

            if (!await dirClient.ExistsAsync()) return false;

            // Get a reference to a file and upload it
            var fileShareClient = dirClient.GetFileClient(fileName);

            if (await fileShareClient.ExistsAsync())
            {
                result = await fileShareClient.DeleteIfExistsAsync();
            }

            return result;
        }

        /// <summary>
        /// Create folder
        /// </summary>
        /// <param name="dirPath">Directory path</param>
        /// <param name="isRecursive">Create folder by hierarchy</param>
        public async Task<bool> CreateFolderAsync(string dirPath, bool isRecursive = false)
        {
            dirPath = dirPath.Replace("\\", "/");

            if (isRecursive)
            {
                CreateHierarchyFolders(dirPath);

                return true;
            }

            var dirClient = _shareClient.GetDirectoryClient(dirPath);

            var dir = await dirClient.CreateIfNotExistsAsync();

            return dir.Value != null;
        }

        /// <summary>
        /// Check folder exists or not
        /// </summary>
        /// <param name="dirPath">Directory path</param>
        public async Task<bool> FolderExistsAsync(string dirPath)
        {
            dirPath = dirPath.Replace("\\", "/");

            var dirClient = _shareClient.GetDirectoryClient(dirPath);

            return await dirClient.ExistsAsync();
        }

        /// <summary>
        /// Delete folder
        /// </summary>
        /// <param name="dirPath">Directory path</param>
        /// <param name="isRecursive">Delete folder by hierarchy</param>
        public async Task<bool> DeleteFolderAsync(string dirPath, bool isRecursive = false)
        {
            dirPath = dirPath.Replace("\\", "/");

            if (isRecursive)
            {
                DeleteHierarchyFolders(dirPath);

                return true;
            }

            var dirClient = _shareClient.GetDirectoryClient(dirPath);

            return await dirClient.DeleteIfExistsAsync();
        }

        #endregion
    }
}