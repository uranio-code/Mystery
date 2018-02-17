using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystery.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MysteryTests
{
    
    [TestClass()]
    public class BaseMysteryTest
    {
        [AssemblyInitialize()]
        public static void ClassInit(TestContext context)
        {
            log4net.Config.XmlConfigurator.Configure();
            context.getMystery().AssemblyRegister.Register(typeof(BaseMysteryTest).Assembly);
        }
    }
}
