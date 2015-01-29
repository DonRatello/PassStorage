using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PassStorageTest
{
    [TestClass]
    public class CryptTest
    {
        [TestMethod]
        public void RandomStringTest()
        {
            var result = PassStorage.Classes.Crypt.GenerateString(10);
            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Length);
        }
    }
}
