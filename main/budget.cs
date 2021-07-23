/* Budget object finally! */
using System;
using System.Data;
using System.IO;
using System.IO.Compression;


// This whole class got dropped
// Budget fields rolled into the accounts table to simplify processing tasks
namespace edhap
{
    /*
        Budget is similar to an account except that wants to add an available component on a per diem basis
        Budget needs to be allocated an amount similar to an account except it takes a direct allocation
        without a posting transaction required, and offsets against a separate budget account for income
        the budget account for income is essentially a posting transaction

        Is adding a table and row for budget fields better than making every account a budget?
        Or better than making budgets fully separate type of account?
        All the processing can work on all accounts if budgets are not a different type of account which is good
        Making all accounts budgets causes no harm though, just doing a select over rows for a datarow collection where budget or true or false would return the split set and avoid having to add a keying field between these and multiple lookups and exra dependencies?
        But that means budget centric code can't be encapsulated away from account centric code? Is there a difference?

        I guess flesh it out and refactor as needed.
    */
    class Budget {
        private db DBase;
        private Accounts AcctTbl;
        private DataTable budgetTbl = null;

        /* 
            Main purpose of account group is to function as a bottom line
            for multiple accounts together
            This is a psuedo-account cannot post transactions to it nor budget to it.
            Merely a holder for the balances of linked accounts
            Any top line account without a parent will reflect an 'out of balance' situation
            Normal for real accounts to have this out of balance as money is waiting to be spent (or overdrawn)
            For budgets the goal should be to ensure the top line balance is 0 always.
        */

        public Budget(db dbase, Accounts acct) {
            // Call the get function once for setup
            this.DBase = dbase;
            AcctTbl = acct;
        }
        
        public DataTable getbudgetTbl() {
            // This function creates a data table and provides it back. Allows this class to work with a data table is has defined while the dataset (db) handler can have this table added for its maintenance and crossreferences
                        // Needs to build data tables
            // Column name convention, lcase start means internal, ucase start means will be displayed and visible directly to user
            // All code paths are meant to set budgetTbl and final return always returns that value
            if (budgetTbl == null) 
            {
                //DBase.dropTbl("accountgroup"); // Comment in to regenerate the table schema, will lose any data in the table
                if (DBase.getTbl("budgets") != null) 
                {
                    // In case the db was reloaded
                    budgetTbl = DBase.getTbl("budgets");
                } else {
                    // In case db does not have a table, it will be created here
                    budgetTbl = new DataTable("budgets");
                    DataColumn acctId = DBase.newCol("budgetId","Int64");
                    acctId.DefaultValue = null;
                    acctId.AutoIncrement = true;
                    budgetTbl.Columns.Add(acctId);
                    DataColumn[] AcctPrimKey = { acctId } ;
                    budgetTbl.PrimaryKey = AcctPrimKey;
                    budgetTbl.Columns.Add(DBase.newCol("clrBal","Double")); 
                    budgetTbl.Columns.Add(DBase.newCol("avlBal","Double"));
                    budgetTbl.Columns.Add(DBase.newCol("windowBudget","Int64")); // Ties to an budgetTbl row
                    budgetTbl.Columns.Add(DBase.newCol("monthBudget","Double"));
                    budgetTbl.Columns.Add(DBase.newCol("perdiem","Boolean"));
                    budgetTbl.Columns.Add(DBase.newCol("acctKey","Int64"));
                    DBase.setTbl(budgetTbl);
                }
            }
            return budgetTbl;
        }
    }
}