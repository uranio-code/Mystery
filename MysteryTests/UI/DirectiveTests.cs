using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystery.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.UI.Tests
{
    [TestClass()]
    public class DirectiveTests
    {
        [TestMethod()]
        public void DirectivegetJsTest()
        {

            var engine = new Jurassic.ScriptEngine();
            engine.Evaluate("var app={};");
            engine.Evaluate("app.directive = function(name,callback){ return callback();};");
            var dir = new MysteryDirective();
            dir.name = "test";
            dir.template = "hello there!";
            string js = dir.getJs();
            engine.Evaluate(js);
            //no crash is good
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void DirectivegetJsScopeTest()
        {

            var engine = new Jurassic.ScriptEngine();
            engine.Evaluate("var app={};");
            engine.Evaluate("app.directive = function(name,callback){ return callback();};");
            var dir = new MysteryDirective();
            dir.name = "test";
            dir.template = "hello there!";
            dir.scopes = "uid,property";
            string js = dir.getJs();
            engine.Evaluate(js);
            //no crash is good
            Assert.IsTrue(true);
        }
        [TestMethod()]
        public void DirectiveGetJsUrlTest()
        {

            var engine = new Jurassic.ScriptEngine();
            engine.Evaluate("var app={};");
            engine.Evaluate("app.directive = function(name,callback){ return callback();};");
            var dir = new MysteryDirective();
            dir.name = "test";
            dir.templateUrl = "ciao.html";
            string js = dir.getJs();
            engine.Evaluate(js);
            //no crash is good
            Assert.IsTrue(true);
        }
        [TestMethod()]
        public void DirectiveGetTemplateHavePriorityTest()
        {

            var engine = new Jurassic.ScriptEngine();
            engine.Evaluate("var app={};");
            engine.Evaluate("app.directive = function(name,callback){ return callback();};");
            var dir = new MysteryDirective();
            dir.name = "test";
            dir.template = "hello there!";
            string js = dir.getJs();
            dir.templateUrl = "ciao.html";
            string js2 = dir.getJs();
            engine.Evaluate(js2);
            Assert.AreEqual(js,js2);
        }

    }
}