using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using Mystery.Configuration;
using Mystery.Content;
using Mystery.Json;
using Mystery.Messaging;
using Mystery.Register;
using Mystery.Users;
using Mystery.Web;
using MysteryWebLogic.Content;
using System.Data.SqlClient;
using System.IO;

namespace IntegrationTests
{

    [TestClass]
    public class IntegrationTestSetup
    {
        // the database to be used for tests. Destroys and creates it ever time we execute tests.
        public static string db_name = "MysteryIntegrationTest";
        public static string db_instance_connection_string;

        

        public static IMongoCollection<BsonDocument> collection;

        [AssemblyInitialize]
        public static void initIntegrationTest(TestContext context)
        {

            var cfp = new MongoDbConfigurationProvider() { env_name = nameof(IntegrationTests) };
            context.setGlobalObject<IConfigurationProvider>(cfp);

            //context.getMystery().AssemblyRegister.Register(typeof(MysteryTests.BaseMysteryTest).Assembly);
            context.getMystery().AssemblyRegister.Register(typeof(MysteryDMS.Model.DMSFolder).Assembly);
            context.getMystery().AssemblyRegister.Register(typeof(WebContentDispatcher).Assembly);

            MysterySession session = context.getGlobalObject<MysterySession>();
            IGlobalContentCreator cc = context.getGlobalObject<IGlobalContentCreator>();
            session.authenticated_user = cc.getNewContent<User>();
            session.authenticated_user.account_type = UserType.admin;

            context.setGlobalObject<IContentDispatcher>(new ContentMemory());
            context.setGlobalObject<INotifier>(new MongoMockNotifier());

            ContentsDatabase db = context.getGlobalObject<ContentsDatabase>();
            db.can_do_crazy = true;
            db.Clear();
            db.Add(session.authenticated_user);
        }

    }
}
