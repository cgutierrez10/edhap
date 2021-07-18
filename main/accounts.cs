using System;
using System.Data;
using System.IO;
using System.IO.Compression;

namespace edhap
{
    class Accounts {
        private DataTable AcctTable = null;
        private db DBase;
        public Accounts(db dbase) {
            // Call the get function once for setup
            this.DBase = dbase;
            AcctTable = getAcctTbl();
        }

        public DataTable getAcctTbl() {
            // This function creates a data table and provides it back. Allows this class to work with a data table is has defined while the dataset (db) handler can have this table added for its maintenance and crossreferences
                        // Needs to build data tables
            // Column name convention, lcase start means internal, ucase start means will be displayed and visible directly to user
            // All code paths are meant to set AcctTable and final return always returns that value
            if (AcctTable == null) 
            {
                if (DBase.getTbl("accounts") != null) 
                {
                    // In case the db was reloaded
                    AcctTable = DBase.getTbl("accounts");
                } else {
                    // In case db does not have a table, it will be created here
                    AcctTable = new DataTable("accounts");
                    DataColumn acctId = DBase.newCol("acctId","Int64");
                    acctId.AutoIncrement = true;
                    AcctTable.Columns.Add(acctId);
                    DataColumn[] AcctPrimKey = { acctId } ;
                    AcctTable.PrimaryKey = AcctPrimKey;
                    AcctTable.Columns.Add(DBase.newCol("Name","String"));
                    AcctTable.Columns.Add(DBase.newCol("budget","Boolean")); // Boolean, budget or real acct
                    AcctTable.Columns.Add(DBase.newCol("Tracking","Boolean"));
                    AcctTable.Columns.Add(DBase.newCol("Balance","Double"));
                    AcctTable.Columns.Add(DBase.newCol("WorkingBal","Double"));
                    AcctTable.Columns.Add(DBase.newCol("parent","Int64")); // Ties to another 'account' type object.
                    AcctTable.Columns.Add(DBase.newCol("Comment","String"));
                    AcctTable.Columns.Add(DBase.newCol("Carryover","Double"));
                    AcctTable.Columns.Add(DBase.newCol("LastUpdate","Int64")); // yy-julian date  indicating last time this account was processed, for catch up purposes.
                    DBase.setTbl(AcctTable);
                }
            }
            return AcctTable;
        }

        public void addAcct(String name){
            DataRow acctrow = getAcct();
            acctrow["Name"] = name;
            //acctrow["acctId"] = 0; // Id auto increments
            acctrow["budget"] = true;
            acctrow["Tracking"] = true;
            acctrow["Balance"] = 0.00;
            acctrow["WorkingBal"] = 0.00;
            acctrow["parent"] = -1;
            acctrow["Comment"] = "";
            acctrow["Carryover"] = 0.00;
            acctrow["LastUpdate"] = 20001; // Jan 1st 2020
            // From this the usual blank account would just be 0'd balanced, no comment, name tracking and lastupdate = today
            setAcct(acctrow);
        }

        public DataRow getAcct(Int64 AcctId = -1) {
            // Returns a new blank row with the correct columns
            return DBase.getRow("accounts",AcctId);
        }

        public Boolean setAcct(DataRow Account) {
            // Received a valid budget type row and commits it, if new then add else update
            // Validate table columns match underlying table
            
            // Should do a column validation here but won't for the time being
            getAcctTbl().Rows.Add(Account);
            return true;
        }

        public Boolean rmAccount(Int64 key = -1) {
            // Stub
            return true;
        }

        // Will want get/sets for all the columns
        // Also a zero out record would be nice for debugging
    }
}