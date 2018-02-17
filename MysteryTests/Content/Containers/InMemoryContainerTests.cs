using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystery.Content;
using MysteryTests.Content.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mystery.Register;
using MysteryTests.Content.Containers;

namespace Mystery.Content.Tests
{
    [TestClass()]
    public class InMemoryContainerTests: ContainerTests
    {

        protected override IContentContainer getContainer()
        {
            return new InMemoryContainer();
        }
    }
    [TestClass()]
    public class ContentContainerCacheTest : ContainerTests
    {

        protected override IContentContainer getContainer()
        {
            return new ContentContainerCache( new InMemoryContainer());
        }
    }
}