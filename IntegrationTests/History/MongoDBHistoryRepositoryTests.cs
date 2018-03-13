using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystery.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.History.Tests
{
    [TestClass()]
    public class MongoDBHistoryRepositoryTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            var repo = new MongoDBHistoryRepository();
            repo.DropAllHistory();
        }

        [TestMethod()]
        public void MongoDBHistoryRepositoryGetByFilterTest()
        {
            var repo = new MongoDBHistoryRepository();
            repo.Add(new HistoryEntry() { id = Guid.NewGuid() });
            repo.Add(new HistoryEntry() { id = Guid.NewGuid() });

            Assert.AreEqual(2,repo.GetByFilter(x=>true).Count());
        }

        [TestMethod()]
        public void MongoDBHistoryRepositoryGetByFilterTest2()
        {
            var guids = new List<Guid>() { Guid.NewGuid(), Guid.NewGuid() };
            var repo = new MongoDBHistoryRepository();
            foreach(var guid in guids)
                repo.Add(new HistoryEntry() { id = guid});
            //one more to check
            repo.Add(new HistoryEntry() { id = Guid.NewGuid() });

            Assert.AreEqual(2, repo.GetByFilter(x => guids.Contains(x.id) ).Count());
        }
    }
}