using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystery.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.FileRepository.Tests
{
    [TestClass()]
    public class MongoFsFileRepositoryTests
    {

        private Stream getProbe(string content) {
            if (string.IsNullOrEmpty(content))
                content = "hello there!";
            var result = new MemoryStream();
            var bytes = content.getBytes();
            result.Write(bytes, 0, bytes.Length);
            result.Position = 0;
            return result;
        }

        [TestMethod()]
        public void MongoFsFileRepositoryMongoFsFileRepositoryTest()
        {
            var repo = new MongoFsSingleCopyFileRepository();
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void MongoFsFileRepositoryCloneFileTest()
        {
            var repo = new MongoFsSingleCopyFileRepository();
            var orginal = repo.CreateFile("txt", getProbe("1"));
            var clone = repo.CloneFile(orginal);
            repo.DeleteFile(orginal);
            Assert.IsTrue(repo.GetFileStream(clone).Length>0);
        }

        [TestMethod()]
        public void MongoFsFileRepositoryCreateFileTest()
        {
            var repo = new MongoFsSingleCopyFileRepository();
            var bytes = new byte[] { 1, 2, 3 };
            var orginal = repo.CreateFile("bin", bytes);
            Assert.IsTrue(repo.GetFileBytes(orginal).SequenceEqual(bytes));
        }

        [TestMethod()]
        public void MongoFsFileRepositoryCreateFileTest1()
        {
            var repo = new MongoFsSingleCopyFileRepository();
            var content = Guid.NewGuid().ToString();
            var orginal = repo.CreateFile("txt", getProbe(content));
            using (var r = new StreamReader(repo.GetFileStream(orginal))) {
                Assert.IsTrue(content == r.ReadToEnd());
            }
        }

        [TestMethod()]
        public void MongoFsFileRepositoryCreateTmpFileTest()
        {
            var repo = new MongoFsSingleCopyFileRepository();
            var bytes = new byte[] { 1, 2, 3 };
            var orginal = repo.CreateTmpFile("bin",DateTime.Now.AddDays(1),  bytes);
            Assert.IsTrue(repo.GetFileBytes(orginal).SequenceEqual(bytes));
        }

        [TestMethod()]
        public void MongoFsFileRepositoryCreateTmpFileTest1()
        {
            var repo = new MongoFsSingleCopyFileRepository();
            var content = Guid.NewGuid().ToString();
            var orginal = repo.CreateTmpFile("txt", DateTime.Now.AddDays(1), getProbe(content));
            using (var r = new StreamReader(repo.GetFileStream(orginal)))
            {
                Assert.IsTrue(content == r.ReadToEnd());
            }
        }

        [TestMethod()]
        public void MongoFsFileRepositoryDeleteFileTest()
        {
            var repo = new MongoFsSingleCopyFileRepository();
            var orginal = repo.CreateFile("txt", getProbe("1"));
            repo.DeleteFile(orginal);
            Assert.IsNull(repo.GetFileBytes(orginal));
        }

    }
}