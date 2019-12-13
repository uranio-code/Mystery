using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystery.Register;
using System.Data.SqlClient;
using System.IO;
using MysteryWebLogic.Content;
using MysteryWebLogic.Authetication;
using Mystery.Content;
using MysteryTests.Content.Definition;
using Mystery.MysteryAction;
using Mystery.Json;
using Mystery.Users;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using Mystery.Configuration;
using Mystery.Web;

namespace IntegrationTests
{

    

    [TestClass]
    public class DatabaseTest
    {

        
        private class CreateAContentInDb<ContentType>: BaseMysteryAction<Guid> where ContentType:IContent
        {
            protected override ActionResult<Guid> ActionImplemetation()
            {
                IGlobalContentCreator cc = this.getGlobalObject<IGlobalContentCreator>();
                IContent c = cc.getNewContent<TestContentType>();
                IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
                cd.Add(c);
                return new ActionResult<Guid>(c.guid);
            }

            protected override bool AuthorizeImplementation()
            {
                return true;
            }
        }

        private class SaveAContentInDb<ContentType> : BaseMysteryAction<ContentType, ContentType> where ContentType : IContent
        {
            protected override ActionResult<ContentType> ActionImplemetation()
            {
                IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
                cd.Add(this.input);
                return new ActionResult<ContentType>(this.input);
            }

            protected override bool AuthorizeImplementation()
            {
                return true;
            }
        }

        [TestMethod]
        public void CreateAContentInDbTest()
        {
            Guid guid = Guid.Empty;
            
            using (WebActionExecutor e = new WebActionExecutor())
            {
                string json = e.executeAction(new CreateAContentInDb<TestContentType>()).json_output;
                IMysteryJsonConverter converter = this.getGlobalObject<IMysteryJsonConverter>();
                guid = converter.readJson<Guid>(json);
            }
            
            ContentsDatabase db = this.getGlobalObject<ContentsDatabase>();
            Assert.IsNotNull(db.GetContent<TestContentType>(guid));
            Assert.IsTrue(db.ContainsType<TestContentType>());
        }

 

        [TestMethod]
        public void CreateAContentEditItandDeleteITInDbTest()
        {
            Guid guid = Guid.Empty;

            using (WebActionExecutor e = new WebActionExecutor())
            {
                string json = e.executeAction(new CreateAContentInDb<TestContentType>()).json_output;
                IMysteryJsonConverter converter = this.getGlobalObject<IMysteryJsonConverter>();
                guid = converter.readJson<Guid>(json);
            }
            using (WebActionExecutor e = new WebActionExecutor())
            {
                IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
                TestContentType c = cd.GetContent<TestContentType>(guid);
                c.a_string = "Carlo";
                e.executeAction(new SaveAContentInDb<TestContentType>(), c);
            }

            Assert.IsTrue(true);
        }
    }
}
