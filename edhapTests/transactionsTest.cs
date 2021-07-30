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
            // Table blank dataset counts should all be 0
            Assert.AreEqual(0, testObjBlank.Count(), "Transaction table expected count 0 initalized with data");
            Assert.NotZero(testObjPop.Count(), "Transaction table expected count 0 initalized with data");
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
            Assert.True((testObj.addTrans(0,-1,10.00,2020001) == -1),"Did not graceful reject add trans with invalid acct (v1)");
            Assert.True((testObj.addTrans(-1,0,10.00,2020001) == -1),"Did not graceful reject add trans with invalid acct(v2)");
            Assert.True((testObj.addTrans(-1,-1,10.00,2020001) == -1),"Did not graceful reject add trans with invalid acct(v3)");
            Assert.AreEqual(testVal,testObj.Count(),"Add transaction failed but count increased");
            // Should also fail if accounts are the same
            Assert.AreEqual(-1,testObj.addTrans(0,0,10.00,2020001),"Did not reject transaction where accounts match");
            // And should fail is accounts are same budget/type
            // 0 is false, 1 true, 2 true, 3 false, Accounts must be cross-type, a true and a false
            Assert.AreEqual(-1,testObj.addTrans(1,2,10.00,2020001),"Did not reject transaction where account types match (v1)");
            Assert.AreEqual(-1,testObj.addTrans(0,3,10.00,2020001),"Did not reject transaction where account types match (v2)");
            Assert.AreEqual(-1,testObj.addTrans(2,1,10.00,2020001),"Did not reject transaction where account types match (v3)");
            Assert.AreEqual(-1,testObj.addTrans(3,0,10.00,2020001),"Did not reject transaction where account types match (v4)");

            // Should be Fails! Invalid accounts
            Assert.True((testObj.addTrans(0,1,10.00,20001) > ((Int64) (-1))),"Valid add failed");
            Assert.AreEqual((testVal + 1),testObj.Count(),"Add transaction succeeded but count changed by more than +1");
        }

        [Test]
        public void transactions_sumTransTest() {
            // Designing tests now for methods that have not been written yet. Getting ahead of the code as unit testing should be done.

            // Call comes in 2 overloads, one takes account and 3 optional, the other takes account and cleared/not and 2 optional and wrap-calls the 1 +3 form

            // Planning several method calls, account, account:startdt, account:startdt:enddt
            // Is the right thing here return a result set, or resturning a date/acct/amt set and having reconcilliation handle the account balances?
            // This is a hobby project, lets not overthink it and if I rewrite I rewrite.
            Assert.Zero(testObjBlank.sumTrans(0,2020002,2020002),"Transaction sum from date to date expecting 0 returned not zero (v1)");
            Assert.NotZero(testObjPop.sumTrans(0),"Transaction sum returned 0.00");
            Assert.Zero(testObjPop.sumTrans(-1),"Transaction sum on invalid account should be 0.00"); // Invalid account may cause internal querying to fail
            Assert.NotZero(testObjPop.sumTrans(0,2020001),"Transaction sum from date returned 0.00");
            Assert.Zero(testObjPop.sumTrans(0,2020002,2020002),"Transaction sum from date to date expecting 0 returned not zero (v2)");
            Assert.NotZero(testObjPop.sumTrans(0,2020001,2020005),"Transaction sum from date to date returned 0.00");
            
            //Account test data at present returns values of 2.55 (since they are binary based increments and I used 8 of them)
            // Can write tests that use account working balance against summation for 'whole data' sum tests
            Assert.AreEqual(AcctPop.getWorkingBal(0),testObjPop.sumTrans(0),0.000001,"Balance from whole history sum did not match account expected (v2)");
            Assert.AreEqual(AcctPop.getCurBal(0),testObjPop.sumTrans(0,((Boolean) true)),0.000001,"Balance from whole history sum did not match account expected (v1)");

            // Data set needs transactions which have forward and backward directions
            // Data set needs transactions which link across multiple different accounts
            // Data set needs transactions which are/not cleared
            // Data set needs transactions which are/not reconciled
        }

    }
}