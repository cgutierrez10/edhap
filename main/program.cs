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

            trans.addTrans(1,2,10.09,20001);
            trans.addTrans(1,2,-10.09,20001);
            trans.addTrans(2,1,9.10,20001);
            trans.addTrans(1,1,0.00,20002);

            Console.WriteLine("Test");
            databaseObj.saveDb("test.xml.gz");
        }
    }
}
