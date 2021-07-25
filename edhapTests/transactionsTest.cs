using NUnit.Framework;
using System;
using System.Data;
using edhap;
using NUnit;

namespace edhapTests
{
    public class transactionsTest {
        Transactions testObj;
        Accounts Acct;
        AccountGroup AcctGrp;
        
        [SetUp]
        public void Setup()
        {
            db databaseObj = new db("test.xml");
            AcctGrp = new AccountGroup(databaseObj);
            Acct = new Accounts(databaseObj, AcctGrp);
            testObj = new Transactions(databaseObj,Acct);
            /*
            AcctGrp.createAcctGrp("test",-1,false);
            AcctGrp.createAcctGrp("test",-1,true);
            Acct.addAcct("test budget", 0, false);
            Acct.addAcct("test real", 1, true);
            */
        }

        [Test]
        public void transactionsConfigTest()
        {
            db databaseObj = new db();
            AccountGroup AcctGrp = new AccountGroup(databaseObj);
            Accounts Acct = new Accounts(databaseObj, AcctGrp);
            Transactions Trans = new Transactions(databaseObj,Acct);

            // Initializes blank worked
            Assert.NotNull(Trans, "Transaction table initialized");
            // Table counts should all be 0
            Assert.AreEqual(0, Trans.Count(), "Transaction table expected count 0 initalized with data");

            //databaseObj = null;
            //databaseObj = new db("unittest.xml");
            //Trans = new Transactions(databaseObj,Acct);
            // Will anticipate about a score of transactions for testing at this point
            //Assert.AreEqual(20, Trans.Count(), "Transaction table expected count 0 initalized with data");
        }

        [Test]
        public void transactions_addTransTest() {
            // Testing that a transaction is added
            // Do a get on a new blank, then set, then another get on that id and check for accuracy
            // Check that transaction count is prior, now +1

            // Lets do an overload to 
            testObj.addTrans(0,1,10.00,21001);
            DataRow testRow = testObj.getTrans(0);

            int transCount = testObj.Count();
            //Graceful fail on invalid account id's?
            Assert.True((testObj.addTrans(0,-1,10.00,21001) == -1),"Did not graceful and correctly fail add trans invalid acct");
            Assert.AreEqual(transCount,testObj.Count(),"Add transaction failed but count increased");


            // Fails! Invalid accounts?
            Int64 testVal = testObj.addTrans(0,1,10.00,21001); // Testval is transid
            Assert.True((testVal > ((Int64) (-1))),"Valid add failed: " + testVal);
            Assert.AreEqual((transCount + 1),testObj.Count(),"Add transaction succeeded but count changed by more than +1");
        }

        // Basic testing
        // A table is created, has correct rows
        // On creation, table is empty
        // On call to add transaction table contains +1 rows (when returning true)
        // On call tao add transaction table contains +0 rows (when returning false)
        // On call to add transaction table, fails when invalid conditions given (accounts incorrect, date invalid?)
        // Incorrect accounts form would be 2 accounts with same budget/real value.
        // Otherwise if accounts are different types then should correct these and be +1 rows

        // Also, should validate account trying to link to actually exists (ie give invalid account and see what happens, handle correctly)

    }
}