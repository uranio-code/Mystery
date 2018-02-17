using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using Mystery.Configuration;
using Mystery.Content;
using Mystery.Files;
using Mystery.History;
using Mystery.Json;
using Mystery.Messaging;
using Mystery.Register;
using Mystery.Users;
using Mystery.Web;
using MysteryTests.Content.Definition;
using MysteryWebLogic.Content;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests
{
    
    [TestClass]
    public class IntegrationTestSetup
    {
        

        public static IMongoCollection<BsonDocument> collection;

        [AssemblyInitialize]
        public static void initIntegrationTest(TestContext context)
        {

            var cfp = new MongoDbConfigurationProvider() { env_name = nameof(IntegrationTests)};
            context.setGlobalObject<IConfigurationProvider>(cfp);

            context.getMystery().AssemblyRegister.Register(typeof(MysteryTests.BaseMysteryTest).Assembly);
            context.getMystery().AssemblyRegister.Register(typeof(DatabaseTest).Assembly);
            context.getMystery().AssemblyRegister.Register(typeof(WebContentDispatcher).Assembly);

            MysterySession session = context.getGlobalObject<MysterySession>();
            IGlobalContentCreator cc = context.getGlobalObject<IGlobalContentCreator>();
            session.authenticated_user = cc.getNewContent<User>();
            session.authenticated_user.account_type = UserType.admin;

            context.setGlobalObject<IContentDispatcher>(new ContentMemory());
            context.setGlobalObject<INotifier>(new MongoMockNotifier());

            ContentsDatabase db = context.getGlobalObject<ContentsDatabase>();
            db.can_do_crazy = true;
            db.Add(session.authenticated_user);
            IContent c = cc.getNewContent<TestContentType>();
            db.Add(c);
            db.Clear();

            //cleaning for tests
            var test_folder_files = context.getGlobalObject<IConfigurationProvider>()
                .getConfiguration<MongoFsFileRepositoryConfiguration>()
                .files_root_folder;

            if (test_folder_files.Exists) {
                foreach (var sub_folder in test_folder_files.GetDirectories()) {
                    sub_folder.Delete(true);
                }
            }

            var content_db = context.getGlobalObject<MysteryMongoDb>().content_db;

            content_db.DropCollection(nameof(MongoFsSingleCopyFileRepository));

            content_db.DropCollection(MongoDBHistoryRepository.collectionName);


            var client = new MongoClient();
            client.DropDatabase("testDb");
            var mongo_db = client.GetDatabase("testDb");
            collection = mongo_db.GetCollection<BsonDocument>(nameof(TestContentType));
            var converter = context.getGlobalObject<MysteryJsonConverter>();
            collection.InsertOne(BsonDocument.Parse(converter.getJson(c)));
        }

        [TestInitialize()]
        public void Initialize()
        {
            var repo = new MongoDBHistoryRepository();
            repo.DropAllHistory();
        }

    }
}
