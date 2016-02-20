namespace Blazer.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Blazer;

    [TestClass]
    public class ParameterFactoryFactoryTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var fac = ParameterFactoryFactory.GetFactory(new { Id = 42, Name = "Bart" });
        }
    }
}
