using NUnit.Framework;
using edhap;

namespace edhapTests
{
    public class transactionsTest {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void transactionsConfigTest()
        {
            db databaseObj = new db();
            AccountGroup AcctGrp = new AccountGroup(databaseObj);
            Accounts Acct = new Accounts(databaseObj, AcctGrp);
            Transactions Trans = new Transactions(databaseObj,Acct);

            Assert.NotNull(Trans);
            
        }

        // Basic testing
        // A table is created, has correct rows
        // On creation, table is empty
        // On call to add transaction table contains +1 rows (when returning true)
        // On call to add transaction table contains +0 rows (when returning false)
        // On call to add transaction table, fails when invalid conditions given (accounts incorrect, date invalid?)
        // Incorrect accounts form would be 2 accounts with same budget/real value.
        // Otherwise if accounts are different types then should correct these and be +1 rows

        // Also, should validate account trying to link to actually exists (ie give invalid account and see what happens, handle correctly)

    }
}