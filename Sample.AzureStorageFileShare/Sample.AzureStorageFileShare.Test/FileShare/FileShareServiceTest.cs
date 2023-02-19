using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample.AzureStorageFileShare.FileShare;

namespace Sample.AzureStorageFileShare.Test.FileShare
{
    [TestClass]
    public class FileShareServiceTest
    {
        private readonly FileShareService _service;

        private readonly string _azureStorageConnectionString =
            @"[Your storage account connection string]";

        private readonly string _shareName = "sharetest";

        public FileShareServiceTest()
        {
            _service = new FileShareService(
                _azureStorageConnectionString,
                _shareName
            );
        }

        [TestMethod]
        [Description("Test UploadAsFileAsync with bytes: Check file share writing success or not")]
        public void UploadAsFileAsyncWithBytesTest()
        {
            var dirPath = $"\\{nameof(UploadAsFileAsyncWithBytesTest)}\\test1\\test2";
            var fileName = $"{DateTime.Now:yyyyMMddHHmmss}_test.txt";
            var isExists = false;

            try
            {
                var dataBytes = Encoding.UTF8.GetBytes(
                    @"[{""Test1"":""test1""},{""Test2"":""test2""},{""Test3"":""test3""}]"
                );

                _service.UploadAsFileAsync(dirPath, fileName, dataBytes)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                isExists = _service.FileExistsAsync(dirPath, fileName).ConfigureAwait(false).GetAwaiter()
                    .GetResult();

                Assert.IsTrue(isExists);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                Assert.IsTrue(false);
            }
            finally
            {
                if (isExists)
                    if (_service.DeleteFileAsync(dirPath, fileName).ConfigureAwait(false).GetAwaiter().GetResult())
                        _service.DeleteFolderAsync(dirPath, true).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }
        [TestMethod]
        [Description("Test UploadAsFileAsync with stream: Check file share writing success or not")]
        public void UploadAsFileAsyncWithStreamTest()
        {
            var dirPath = $"\\{nameof(UploadAsFileAsyncWithStreamTest)}\\test1\\test2";
            var fileName = $"{DateTime.Now:yyyyMMddHHmmss}_test.txt";
            var isExists = false;

            try
            {
                var dataBytes = Encoding.UTF8.GetBytes(
                    @"[{""Test1"":""test1""},{""Test2"":""test2""},{""Test3"":""test3""}]"
                );

                var stream = new MemoryStream(dataBytes);

                _service.UploadAsFileAsync(dirPath, fileName, stream)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                isExists = _service.FileExistsAsync(dirPath, fileName).ConfigureAwait(false).GetAwaiter()
                    .GetResult();

                Assert.IsTrue(isExists);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                Assert.IsTrue(false);
            }
            finally
            {
                if (isExists)
                    if (_service.DeleteFileAsync(dirPath, fileName).ConfigureAwait(false).GetAwaiter().GetResult())
                        _service.DeleteFolderAsync(dirPath, true).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        [TestMethod]
        [Description("Test FileExistsAsync: Check file exists or not")]
        public void FileExistsAsyncTest()
        {
            var dirPath = $"\\{nameof(FileExistsAsyncTest)}\\test1\\test2";
            var fileName = $"{DateTime.Now:yyyyMMddHHmmss}_test.txt";
            var isExists = false;

            try
            {
                var dataBytes = Encoding.UTF8.GetBytes(
                    @"[{""Test1"":""test1""},{""Test2"":""test2""},{""Test3"":""test3""}]"
                );

                _service.UploadAsFileAsync(dirPath, fileName, dataBytes)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                isExists = _service.FileExistsAsync(dirPath, fileName).ConfigureAwait(false).GetAwaiter()
                    .GetResult();

                Assert.IsTrue(isExists);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                Assert.IsTrue(false);
            }
            finally
            {
                if (isExists)
                    if (_service.DeleteFileAsync(dirPath, fileName).ConfigureAwait(false).GetAwaiter().GetResult())
                        _service.DeleteFolderAsync(dirPath, true).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        [TestMethod]
        [Description("Test DeleteFileAsync: Delete file")]
        public void DeleteFileAsyncTest()
        {
            var dirPath = $"\\{nameof(DeleteFileAsyncTest)}\\test1\\test2";
            var fileName = $"{DateTime.Now:yyyyMMddHHmmss}_test.txt";

            try
            {
                var dataBytes = Encoding.UTF8.GetBytes(
                    @"[{""Test1"":""test1""},{""Test2"":""test2""},{""Test3"":""test3""}]"
                );

                _service.UploadAsFileAsync(dirPath, fileName, dataBytes)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                var deleteFileResult = _service.DeleteFileAsync(dirPath, fileName).ConfigureAwait(false).GetAwaiter().GetResult();

                if (deleteFileResult)
                    _service.DeleteFolderAsync(dirPath, true).ConfigureAwait(false).GetAwaiter().GetResult();

                Assert.IsTrue(deleteFileResult);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        [Description("Test CreateFolderAsync: Create folder")]
        public void CreateFolderAsyncTest()
        {
            const string dirPath = $"\\{nameof(CreateFolderAsyncTest)}\\test1\\test2";

            try
            {
                _service.CreateFolderAsync(dirPath, true).ConfigureAwait(false).GetAwaiter().GetResult();

                Assert.IsTrue(_service.FolderExistsAsync(dirPath).ConfigureAwait(false).GetAwaiter().GetResult());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                Assert.IsTrue(false);
            }
            finally
            {
                if (_service.FolderExistsAsync(dirPath).ConfigureAwait(false).GetAwaiter().GetResult())
                    _service.DeleteFolderAsync(dirPath, true).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        [TestMethod]
        [Description("Test FolderExistsAsync: Check folder exists or not")]
        public void FolderExistsAsyncTest()
        {
            const string dirPath = $"\\{nameof(FolderExistsAsyncTest)}\\test1\\test2";

            try
            {
                _service.CreateFolderAsync(dirPath, true).ConfigureAwait(false).GetAwaiter().GetResult();

                Assert.IsTrue(_service.FolderExistsAsync(dirPath).ConfigureAwait(false).GetAwaiter().GetResult());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                Assert.IsTrue(false);
            }
            finally
            {
                if (_service.FolderExistsAsync(dirPath).ConfigureAwait(false).GetAwaiter().GetResult())
                    _service.DeleteFolderAsync(dirPath, true).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        [TestMethod]
        [Description("Test DeleteFolderAsync: Delete folder successful or not")]
        public void DeleteFolderAsyncTest()
        {
            const string dirPath = $"\\{nameof(DeleteFolderAsyncTest)}\\test1\\test2";

            try
            {
                _service.CreateFolderAsync(dirPath, true).ConfigureAwait(false).GetAwaiter().GetResult();

                _service.DeleteFolderAsync(dirPath, true).ConfigureAwait(false).GetAwaiter().GetResult();

                Assert.IsTrue(!_service.FolderExistsAsync(dirPath).ConfigureAwait(false).GetAwaiter().GetResult());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                Assert.IsTrue(false);
            }
        }
    }
}