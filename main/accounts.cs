using System;
using System.Data;
using System.IO;
using System.IO.Compression;

namespace edhap
{
    public class Accounts {
        private DataTable AcctTable = null;
        private AccountGroup AcctGrpTbl = null;
        private db DBase;
        public Accounts(db dbase, AccountGroup AcctGrp) {
            // Call the get function once for setup
            this.DBase = dbase;
            AcctTable = getAcctTbl();
            AcctGrpTbl = AcctGrp;
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
                    acctId.DefaultValue = null;
                    acctId.AutoIncrement = true;
                    acctId.ReadOnly = true;
                    AcctTable.Columns.Add(acctId);
                    DataColumn[] AcctPrimKey = { acctId } ;
                    AcctTable.PrimaryKey = AcctPrimKey;
                    AcctTable.Columns.Add(DBase.newCol("Name","String"));
                    AcctTable.Columns.Add(DBase.newCol("budget","Boolean")); // Boolean, budget or real acct
                    AcctTable.Columns.Add(DBase.newCol("Tracking","Boolean"));
                    AcctTable.Columns.Add(DBase.newCol("Balance","Double"));
                    AcctTable.Columns.Add(DBase.newCol("WorkingBal","Double"));
                    AcctTable.Columns.Add(DBase.newCol("parent","Int64")); // Ties to an acctgrptbl row
                    AcctTable.Columns.Add(DBase.newCol("Comment","String"));
                    AcctTable.Columns.Add(DBase.newCol("Carryover","Double"));
                    AcctTable.Columns.Add(DBase.newCol("LastUpdate","Int64")); // yy-julian date  indicating last time this account was processed, for catch up purposes.
                    // These values are used for budget accounts only
                    AcctTable.Columns.Add(DBase.newCol("clrBal","Double")); 
                    AcctTable.Columns.Add(DBase.newCol("avlBal","Double"));
                    AcctTable.Columns.Add(DBase.newCol("windowBudget","Double"));
                    AcctTable.Columns.Add(DBase.newCol("monthBudget","Double"));
                    AcctTable.Columns.Add(DBase.newCol("perdiem","Boolean"));
                    DBase.setTbl(AcctTable);
                }
            }
            return AcctTable;
        }

        public Boolean addAcct(String name, Int64 acctgroup, Boolean budget){
            DataRow acctrow = getAcct();
            acctrow["Name"] = name;
            //acctrow["acctId"] = 0; // Id auto increments
            acctrow["budget"] = budget;
            acctrow["Tracking"] = true;
            //acctrow["Balance"] = 0.00; // Default is fine
            //acctrow["WorkingBal"] = 0.00; // Default is fine
            acctrow["parent"] = acctgroup;
            //acctrow["Comment"] = ""; // Default is fine
            //acctrow["Carryover"] = 0.00; // Default is fine
            acctrow["LastUpdate"] = 20001; // Jan 1st 2020
            //acctrow["clrBal"] = 0.00; // Default is fine
            //acctrow["avlBal"] = 0.00; // Default is fine
            //acctrow["windowBudget"] = 0.00; // Default is fine
            //acctrow["monthBudget"] = 0.00; // Default is fine
            //acctrow["perdiem"] = 0.00; // Default is fine
            // From this the usual blank account would just be 0'd balanced, no comment, name tracking and lastupdate = today
            return setAcct(acctrow);
        }

        public DataRow getAcct(Int64 AcctId = -1) {
            // Returns a new blank row with the correct columns
            return DBase.getRow("accounts",AcctId);
        }

        public Boolean setAcct(DataRow Account) {
            // Received a valid accounts type row and commits it, if new then add else update
            // Validate table columns match underlying table
            
            // Should do a column validation here but won't for the time being
            if ((Boolean) Account["budget"] != (Boolean) AcctGrpTbl.getAcctGrp((Int64) Account["parent"])["budget"])
            {
                return false;
            }
            getAcctTbl().LoadDataRow(Account.ItemArray, LoadOption.PreserveChanges);
            return getAcctTbl().Rows.Contains(Account["acctId"]); // If successfully added then the table will now contain this record.
        }

        public Boolean rmAccount(Int64 key = -1) {
            // Stub
            // Account deletion isn't always valid, history and present data may require it.
            // Only way to do an account deletion would be to move all transactions to another account
            return true;
        }

        // Will want get/sets for all the columns
        // Also a zero out record would be nice for debugging
        public Boolean getBudget(Int64 acct)
        {
            return (Boolean) getAcct(acct)["budget"];
        }

        public void updateWorkBal(Int64 acct, Double balance, Boolean direction) {
            // All balances are positive, direction true = + else -
            DataRow account = AcctTable.NewRow();
            account = getAcct(acct);
            Double bal = Double.Parse(account["WorkingBal"].ToString());
            bal += (direction == (Boolean) true) ? balance : balance * -1;
            account["WorkingBal"] = bal;
            setAcct(account);
        }
        public void updateCurBal(Int64 acct, Double balance, Boolean direction) {
            // All balances are positive, direction true = + else -
            DataRow account = AcctTable.NewRow();
            account = getAcct(acct);
            Double bal = Double.Parse(account["Balance"].ToString());
            bal += (direction == (Boolean) true) ? balance : balance * -1;
            account["Balance"] = bal;
            setAcct(account);
        }
    }
}