using Microsoft.VisualStudio.TestTools.UnitTesting;
using PFire;
using PFire.Session;
using System;


namespace PFire.Tests
{
    [TestClass()]
    public class UT_PFireServerTests
    {
        [TestMethod()]
        public void PFireServer_InitializationTests()
        {
            PFireServer _testServer = new PFireServer(@"C:\Temp");
            Assert.AreEqual(_testServer.Database.DatabasePath,
                @"C:\Temp\pfiredb");
        }

        [TestMethod()]
        public void PFireServer_GetSessionTest()
        {
            PFireServer _testServer = new PFireServer(@"C:\Temp");
            _testServer.Start();
            XFireClient _xFireClient = _testServer.GetSession(new Database.User()
            {
                Username = "b",
                Password = "71fa140c7fa2fff09ff4de0a1a273510b93013c6"
            });
            Assert.AreEqual(_xFireClient, null);
        }
    }
}