using NUnit.Framework;

namespace edhapTests
{
    public class reconcilationTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        // What are my tests goals.
        // The workflow for a reconcilation processing step should be valid.
        // Individual subcomponents must be valid.
        // Transaction steps must be valid
        // Then linkages between records should all be valid (account, account groups, transaction accounts)

    }
}