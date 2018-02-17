using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystery.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mystery.Register;
using Mystery.Json;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mystery.Configuration.Tests
{
    [TestClass()]
    public class MongoDbConfigurationProviderTests
    {

        public class MyTestConf {
            public string my_string { get; set; }
        }


        private static void cleanTestCollection() {
            var o = new object();
            var collection = o.getGlobalObject<MysteryMongoDb>().local_db
                .GetCollection<BsonDocument>(MongoDbConfigurationProvider.collectionName);
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq(nameof(MongoDbConfigurationProvider.env_name), nameof(MongoDbConfigurationProviderTests));
            collection.DeleteMany(filter);
        }


        [ClassInitialize()]
        public static void InitializeMongoDbConfigurationProviderTests(TestContext context) {
            cleanTestCollection();
        }

        [ClassCleanup()]
        public static void CleanupMongoDbConfigurationProviderTests()
        {
            cleanTestCollection();
        }


        [TestMethod()]
        public void MongoDbConfigurationProviderMongoDbConfigurationProviderTest()
        {
            var provider = new MongoDbConfigurationProvider() { env_name = nameof(MongoDbConfigurationProviderTests) };
            var rnd = Guid.NewGuid().ToString();
            provider.getConfiguration<MyTestConf>().my_string = rnd;
            //if I set it should stay without save
            Assert.AreEqual(provider.getConfiguration<MyTestConf>().my_string,rnd);
        }

        [TestMethod()]
        public void MongoDbConfigurationProvidergetConfigurationTest()
        {
            cleanTestCollection();
            var provider = new MongoDbConfigurationProvider() { env_name = nameof(MongoDbConfigurationProviderTests) };
            var rnd = Guid.NewGuid().ToString();
            provider.getConfiguration<MyTestConf>().my_string = rnd;
            provider = new MongoDbConfigurationProvider() { env_name = nameof(MongoDbConfigurationProviderTests) };
            Assert.IsTrue(string.IsNullOrEmpty(provider.getConfiguration<MyTestConf>().my_string));
        }

        [TestMethod()]
        public void MongoDbConfigurationProvidersetConfigurationTest()
        {
            cleanTestCollection();
            var provider = new MongoDbConfigurationProvider() { env_name = nameof(MongoDbConfigurationProviderTests) };
            var rnd = Guid.NewGuid().ToString();
            var conf = provider.getConfiguration<MyTestConf>();
            conf.my_string = rnd;
            provider.setConfiguration(conf);
            provider = new MongoDbConfigurationProvider() { env_name = nameof(MongoDbConfigurationProviderTests) };
            //if I saved it should stay after save
            Assert.AreEqual(provider.getConfiguration<MyTestConf>().my_string, rnd);
        }
    }
}