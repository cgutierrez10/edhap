using System;
using System.Data;

namespace edhap
{
    class Program {
        static void Main(string[] args)
        {
            db databaseObj = new db("test.xml.gz");
            Accounts acct = new Accounts(databaseObj);
            Transactions trans = new Transactions(databaseObj);

            // Sufficient database values completeness with accounts
            /* Sufficient DB completeness
                with accounts and transactions tables only
                to start working on ledger balancing
            */
            Console.WriteLine("Test");
            databaseObj.saveDb("test.xml.gz");
        }
    }
}
