using System;
using System.Data;

namespace edhap
{
    class Program {
        static void Main(string[] args)
        {
            db databaseObj = null;
            if (args.Length == 1 && args[0] == "refresh") {
                databaseObj = new db();
            } else {
                databaseObj = new db("test.xml.gz");
            }
            AccountGroup AcctGrp = new AccountGroup(databaseObj);
            Accounts Acct = new Accounts(databaseObj, AcctGrp);
            Transactions Trans = new Transactions(databaseObj,Acct);


            // From this point it's possible to make a batch file that will create the data tables
            // With the right entries. I can compute from a spreadsheet to get expected  right values
            // Then run the script to do a table setup
            // Then do an output that will throw the spreadsheet data back out for compare
            // Also unit testing soon. Time for that.
            if (args.Length > 0) {
                if (args[0] == "acctgrp" && args.Length == 4) { 
                    Console.WriteLine("Create acctgroup: " + AcctGrp.createAcctGrp(args[1], Int64.Parse(args[2]), args[3] == "true" ? true : false).ToString());
                }
                if (args[0] == "acct" && args.Length == 4) { 
                    Console.WriteLine("Create account: " + Acct.addAcct(args[1], Int64.Parse(args[2]), args[3] == "true" ? true : false).ToString());
                }
                if (args[0] == "trans" && args.Length == 4) { 
                    Console.WriteLine("Create transaction: " + Trans.addTrans(Int64.Parse(args[1]), Int64.Parse(args[2]), Double.Parse(args[3]), 20001).ToString());
                }
            }
            /*
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
            // And set budget values -- Command line this? Hand modify? How to set this.
            // And determine the target results
            // Write the math processing
            // Etc, etc, etc.
            // Possibly best thing to do it provide a command line argument to create each thing then manually set values?

            databaseObj.saveDb("test.xml");
        }
    }
}
