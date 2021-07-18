using System;
using System.Data;
using System.IO;
using System.IO.Compression;

namespace edhap
{
    class Transactions {
        private DataTable TransTable = null;
        private Accounts acct = null;
        private db DBase;
        public Transactions(db dbase, Accounts account) {
            // Call the get function once for setup
            this.DBase = dbase;
            this.acct = account;
            TransTable = getTransTbl();
        }

        public DataTable getTransTbl() {
            // This function creates a data table and provides it back. Allows this class to work with a data table is has defined while the dataset (db) handler can have this table added for its maintenance and crossreferences
                        // Needs to build data tables
            // Column name convention, lcase start means internal, ucase start means will be displayed and visible directly to user
            // All code paths are meant to set TransTable and final return always returns that value
            if (TransTable == null) 
            {
                if (DBase.getTbl("transactions") != null) 
                {
                    // In case the db was reloaded
                    TransTable = DBase.getTbl("transactions");
                } else {
                    // In case db does not have a table, it will be created here
                    DataTable TransTable = new DataTable("transactions");
                    DataColumn transId = DBase.newCol("transId","Int64");
                    transId.AutoIncrement = true;
                    TransTable.Columns.Add(transId);
                    DataColumn[] TransPrimKey = { transId };
                    TransTable.PrimaryKey = TransPrimKey;
                    TransTable.Columns.Add(DBase.newCol("payeeId","Int64")); // Payee will be id keyed, display will pull the correct id-text keying
                    TransTable.Columns.Add(DBase.newCol("Amount","Double"));
                    TransTable.Columns.Add(DBase.newCol("Direction","Boolean"));
                    TransTable.Columns.Add(DBase.newCol("Cleared","Boolean"));
                    TransTable.Columns.Add(DBase.newCol("Reconciled","Boolean"));
                    TransTable.Columns.Add(DBase.newCol("Memo","String"));
                    TransTable.Columns.Add(DBase.newCol("Date","Int64")); // Same as yy-julan date to be used above
                    TransTable.Columns.Add(DBase.newCol("Checknum","String"));
                    TransTable.Columns.Add(DBase.newCol("Realacct","Int64"));
                    TransTable.Columns.Add(DBase.newCol("Budgetacct","Int64"));
                    TransTable.Columns.Add(DBase.newCol("splitkey","Int64")); // splitkey is the parent of split transactions
                    //TransTable.Columns.Add(newCol("trans-id","Int64")); 
                    DBase.setTbl(TransTable);
                }
            }
            return TransTable;
        }

        /*
            More complex construction
            Requires 2 accounts, should validate the accounts one is real one is budget and not the same
            Requires an amount
            date
            as the minimum
        */
        public Boolean addTrans(Int64 acct1, Int64 acct2, Double amt, Int64 dt){
            if (acct.getBudget(acct1) == acct.getBudget(acct2)) {
                return false;
            }

            DataRow Transrow = getTrans();
            //Transrow["TransId"] = 0; // Id auto increments
            Transrow["payeeId"] = -1; // This will be linked later
            Transrow["Amount"] = amt < 0 ? (amt * -1) : amt;
            Transrow["direction"] = amt < 0 ? false : true; // If amount is positive then true else false, positive = + to account balance
            Transrow["Cleared"] = false;
            Transrow["Reconciled"] = false;
            Transrow["Memo"] = "";
            Transrow["Date"] = 20001; // Same Jan 1st yy-julian blank value
            Transrow["Checknum"] = "";
            //Transrow["acctreal"] = -1; // Will normally be required
            //Transrow["acctbudget"] = -1; // Generally the software will enforce real acct != budget account
            Transrow["splitkey"] = -1; // If split transaction, splitkey will be next budget account portion, full balance will sit in primary transaction
            
            if (acct.getBudget(acct1)) {
                Transrow["Budgetacct"] = acct1;
                Transrow["Realacct"] = acct2;
            } else {
                Transrow["Budgetacct"] = acct2;
                Transrow["Realacct"] = acct1;
            }

            setTrans(Transrow);
            return true;
        }

        public DataRow getTrans(Int64 TransId = -1) {
            // Returns a new blank row with the correct columns
            return DBase.getRow("transactions",TransId);
        }

        public Boolean setTrans(DataRow Transaction) {
            // Received a valid budget type row and commits it, if new then add else update
            // Validate table columns match underlying table
            
            // Should do a column validation here but won't for the time being
            getTransTbl().Rows.Add(Transaction);
            return true;
        }

        public Boolean rmTrans(Int64 key = -1) {
            // Stub
            // This should probably be some disable but not delete, or a counter transaction,
            // Generally in accounting one never removes a receipt from the register but reverses it.
            return true;
        }

        // Will want get/sets for all the columns
        // Also a zero out record would be nice for debugging
    }
}