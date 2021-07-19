using System;
using System.Data;

namespace edhap
{
    class Program {
        static void Main(string[] args)
        {
            db databaseObj = new db("test.xml.gz");
            Accounts acct = new Accounts(databaseObj);
            Transactions trans = new Transactions(databaseObj,acct);
            AccountGroup AcctGrp = new AccountGroup(databaseObj,acct);
            /* 
                Sufficient DB completeness
                with accounts and transactions tables only
                to start working on ledger balancing
            */
            /* 
                Alternatively, can move into the gui portion from here 
                That will be a lot of project reconfiguration
                Convert this to a dll and then make a gui in another tool?
            */

            /*
                For now, going into calculations.
                May make this a dll later once it is tested out.
                That would allow a simpler path to android version
                as well as desktop
                and potentially an api tool and web access
            */

            //trans.addTrans(1,2,10.09,20001);
            //trans.addTrans(1,2,-10.09,20001);
            //trans.addTrans(2,1,9.10,20001);
            //trans.addTrans(1,1,0.00,20002);


            /*
                Need 2 more classes before proceeding into transaction processing.
                acctgroup (psuedo-account for other accounts to point to, cannot directly transact to this)
                budget (container for budget specific factors like per diem over interval)

                Processing loop is going to be
                Iterate over transactions, populate working balance (sum of cleared), reconciled balance (sum of cleared and reconciled) and set lastupdate date field
                Complete this across both accounts
                create budget class to generate the per-diem as invisible transactions
                budget class exists for each budget account with extra data and linkage to top lines
                Budget class needs a counter enter (income) for the other side of the dual entry
                Need to create acct containers to group accounting
            */

            // Necessary to create account groups
            // Then accounts
            // Then transactions
            // And set budget values
            // And determine the target results
            // Write the math processing
            // Etc, etc, etc.
            // Possibly best thing to do it provide a command line argument to create each thing then manually set values?

            

            Console.WriteLine("Test");
            databaseObj.saveDb("test.xml.gz");
        }
    }
}
