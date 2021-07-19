using System;
using System.Data;
using System.IO;
using System.IO.Compression;

namespace edhap
{
    class AccountGroup {
        DataTable AcctGrpTbl = null;
        //private Accounts acct = null;
        private db DBase;

        /* 
            Main purpose of account group is to function as a bottom line
            for multiple accounts together
            This is a psuedo-account cannot post transactions to it nor budget to it.
            Merely a holder for the balances of linked accounts
            Any top line account without a parent will reflect an 'out of balance' situation
            Normal for real accounts to have this out of balance as money is waiting to be spent (or overdrawn)
            For budgets the goal should be to ensure the top line balance is 0 always.
        */

        public AccountGroup(db dbase) {
            // Call the get function once for setup
            this.DBase = dbase;
            //this.acct = account;
            AcctGrpTbl = getAcctGrpTbl();
        }
        
        public DataTable getAcctGrpTbl() {
            // This function creates a data table and provides it back. Allows this class to work with a data table is has defined while the dataset (db) handler can have this table added for its maintenance and crossreferences
                        // Needs to build data tables
            // Column name convention, lcase start means internal, ucase start means will be displayed and visible directly to user
            // All code paths are meant to set AcctGrpTbl and final return always returns that value
            if (AcctGrpTbl == null) 
            {
                //DBase.dropTbl("accountgroup"); // Comment in to regenerate the table schema, will lose any data in the table
                if (DBase.getTbl("accountgroup") != null) 
                {
                    // In case the db was reloaded
                    AcctGrpTbl = DBase.getTbl("accountgroup");
                } else {
                    // In case db does not have a table, it will be created here
                    AcctGrpTbl = new DataTable("accountgroup");
                    DataColumn acctId = DBase.newCol("groupId","Int64");
                    acctId.DefaultValue = null;
                    acctId.AutoIncrement = true;
                    AcctGrpTbl.Columns.Add(acctId);
                    DataColumn[] AcctPrimKey = { acctId } ;
                    AcctGrpTbl.PrimaryKey = AcctPrimKey;
                    AcctGrpTbl.Columns.Add(DBase.newCol("Name","String"));
                    AcctGrpTbl.Columns.Add(DBase.newCol("runbal","Double"));
                    AcctGrpTbl.Columns.Add(DBase.newCol("Parent","Int64")); // Ties to an acctgrptbl row
                    AcctGrpTbl.Columns.Add(DBase.newCol("Comment","String"));
                    AcctGrpTbl.Columns.Add(DBase.newCol("Carry","Boolean"));
                    AcctGrpTbl.Columns.Add(DBase.newCol("Budget","Boolean")); // Use this to xref that real accounts link to real acct group and avoid cross-linking budget accounts and cash accounts which could cause issues with bottom line balances
                    AcctGrpTbl.Columns.Add(DBase.newCol("LastUpdate","Int64")); // yy-julian date  indicating last time this account was processed, for catch up purposes.
                    DBase.setTbl(AcctGrpTbl);
                }
            }
            return AcctGrpTbl;
        }

        public Boolean createAcctGrp(String name, Int64 parentacct, Boolean budget)
        {
            DataRow acctgrp = DBase.getRow("accountgroup",-1);
            acctgrp["Name"] = name;
            //acctgrp["runbal"] = 0.00; // Default is fine
            acctgrp["Parent"] = parentacct;
            //acctgrp["Comment"] = ""; // Default is fine
            //acctgrp["Carry"] = false; // Default is fine
            acctgrp["Budget"] = budget;
            acctgrp["LastUpdate"] = 20001;
            return setAcctGrp(acctgrp);
        }

        public Boolean setAcctGrp(DataRow acctgrp) {
            AcctGrpTbl.Rows.Add(acctgrp);
            return AcctGrpTbl.Rows.Contains(acctgrp["groupId"]); // If successfully added then the table will now contain this record.
        }
        public DataRow getAcctGrp(Int64 AcctId = -1) {
            // Returns a new blank row with the correct columns
            return DBase.getRow("accountgroup",AcctId);
        }

        /*  Account processing path
            Update all accounts (iterate over and balance each), accounts object should have a balance() and balanceAll() call
            Then update all account groups now that the accounts are balanced, this must be done as a balanceAll(), no way to update a middle or bottom line if another input group may be out of balance
        */
    }
}
