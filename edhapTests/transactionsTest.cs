using NUnit.Framework;
using System;
using System.Data;
using edhap;
using NUnit;

namespace edhapTests
{
    public class transactionsTest {
        Transactions testObjPop;
        Transactions testObjBlank;
        Accounts AcctPop;
        Accounts AcctBlank;
        AccountGroup AcctGrpPop;
        AccountGroup AcctGrpBlank;

        [SetUp]
        public void Setup()
        {
            db databaseObjPop = new db("test.xml");
            AcctGrpPop = new AccountGroup(databaseObjPop);
            AcctPop = new Accounts(databaseObjPop, AcctGrpPop);
            testObjPop = new Transactions(databaseObjPop,AcctPop);

            db databaseObjBlank = new db();
            AcctGrpBlank = new AccountGroup(databaseObjBlank);
            AcctBlank = new Accounts(databaseObjBlank, AcctGrpBlank);
            testObjBlank = new Transactions(databaseObjBlank,AcctBlank);

        }

        [Test]
        public void transactionsConfigTest()
        {
            // Initializes blank worked
            Assert.NotNull(testObjPop, "Populated Data Transaction table initialized");
            Assert.NotNull(testObjBlank, "Empty data Transaction table initialized");
            // Table counts should all be 0
            Assert.AreEqual(0, testObjBlank.Count(), "Transaction table expected count 0 initalized with data");
        }

        [Test]
        public void transacts_CountTest() {
            // Should definitely verify the count() function works.

            // It's difficult to get a null return, only test code  which count make this fail threw exceptions everywhere anyway
            Assert.NotNull(testObjBlank.Count(),"Transaction.Count() returned a null value"); // This should be impossible, int return type, int is not nullable

            Assert.Zero(testObjBlank.Count(),"Empty dataset transactions.Count() returned non-zero");
            Assert.Positive(testObjPop.Count(),"Populated transactions.Count() returned <1");
        }

        [Test]
        public void transactions_addTransTest() {
            // Testing transaction add success and validation correctness
            // All tests for validation correctness need to test where acct1 and acct2 are swapped. This should always be the same success/fail status.

            Int64 testVal = 0;
            Transactions testObj = testObjPop; // For these tests only using the populated dataset. Accounts required for transactions to succeed.
            Assert.NotNull(testObjBlank.getTrans(0),"getTrans on blank dataset returned null, should always return a row");
            Assert.NotNull(testObjBlank.getTrans(0),"getTrans on populated dataset returned null, should always return a row");

            testVal = testObj.Count();
            //Graceful fail on invalid account id's?
            Assert.True((testObj.addTrans(0,-1,10.00,21001) == -1),"Did not graceful reject add trans with invalid acct (v1)");
            Assert.True((testObj.addTrans(-1,0,10.00,21001) == -1),"Did not graceful reject add trans with invalid acct(v2)");
            Assert.True((testObj.addTrans(-1,-1,10.00,21001) == -1),"Did not graceful reject add trans with invalid acct(v3)");
            Assert.AreEqual(testVal,testObj.Count(),"Add transaction failed but count increased");
            // Should also fail if accounts are the same
            Assert.AreEqual(-1,testObj.addTrans(0,0,10.00,21001),"Did not reject transaction where accounts match");
            // And should fail is accounts are same budget/type
            // 0 is false, 1 true, 2 true, 3 false, Accounts must be cross-type, a true and a false
            Assert.AreEqual(-1,testObj.addTrans(1,2,10.00,21001),"Did not reject transaction where account types match (v1)");
            Assert.AreEqual(-1,testObj.addTrans(0,3,10.00,21001),"Did not reject transaction where account types match (v2)");
            Assert.AreEqual(-1,testObj.addTrans(2,1,10.00,21001),"Did not reject transaction where account types match (v3)");
            Assert.AreEqual(-1,testObj.addTrans(3,0,10.00,21001),"Did not reject transaction where account types match (v4)");

 


            // Fails! Invalid accounts?
            Assert.True((testObj.addTrans(0,1,10.00,21001) > ((Int64) (-1))),"Valid add failed");
            Assert.AreEqual((testVal + 1),testObj.Count(),"Add transaction succeeded but count changed by more than +1");
        }

        // Basic testing
        // A table is created, has correct rows
        // On call to add transaction table, fails when invalid conditions given (accounts incorrect, date invalid?)
        // Incorrect accounts form would be 2 accounts with same budget/real value.
        // Otherwise if accounts are different types then should correct these and be +1 rows

        // Also, should validate account trying to link to actually exists (ie give invalid account and see what happens, handle correctly)

    }
}