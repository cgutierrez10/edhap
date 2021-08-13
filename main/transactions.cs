using System;
using System.Data;
using System.IO;
using System.IO.Compression;

namespace edhap
{
    public class Transactions {
        // TODO: Change all 'acct' method signatures to queryacct to avoid possible conflict with Accounts acct
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
                    transId.DefaultValue = null;
                    transId.AutoIncrement = true;
                    transId.ReadOnly = true;
                    TransTable.Columns.Add(transId);
                    DataColumn[] TransPrimKey = { transId };
                    TransTable.PrimaryKey = TransPrimKey;
                    TransTable.Columns.Add(DBase.newCol("payeeId","Int64")); // Payee will be id keyed, display will pull the correct id-text keying
                    TransTable.Columns.Add(DBase.newCol("Amount","Double"));
                    TransTable.Columns.Add(DBase.newCol("Direction","Boolean"));
                    TransTable.Columns.Add(DBase.newCol("Cleared","Boolean"));
                    TransTable.Columns.Add(DBase.newCol("Reconciled","Boolean"));
                    TransTable.Columns.Add(DBase.newCol("process","Int64")); // Process stages, start at 0 not added to balance, 1 cleared, 2 reconciled to prevent reprocessing/adding?
                    TransTable.Columns.Add(DBase.newCol("Hidden","Boolean")); // Used by budget accounts
                    TransTable.Columns.Add(DBase.newCol("Memo","String"));
                    TransTable.Columns.Add(DBase.newCol("Date","Int64")); // Same as yy-julan date to be used above
                    TransTable.Columns.Add(DBase.newCol("Checknum","String"));
                    TransTable.Columns.Add(DBase.newCol("Realacct","Int64"));
                    TransTable.Columns.Add(DBase.newCol("Budgetacct","Int64"));
                    // Split key should also be read only, but user cannot directly modify it
                    // May be some special situation where splitkey could be modified not locking it readonly yet
                    TransTable.Columns.Add(DBase.newCol("splitkey","Int64")); // splitkey is the parent of split transactions
                    //TransTable.Columns.Add(newCol("trans-id","Int64")); 
                    DBase.setTbl(TransTable);
                }
            }
            return TransTable;
        }

        // Requires 2 accounts, should validate the accounts one is real one is budget and not the same
        // Requires an amount, date
        public Int64 addTrans(Int64 acct1, Int64 acct2, Double amt, Int64 dt){
            DataRow Transrow = TransTable.NewRow();
            Transrow["payeeId"] = -1; // This will be linked later
            Transrow["Amount"] = amt < 0 ? (amt * -1) : amt;
            Transrow["direction"] = amt < 0 ? false : true; // If amount is positive then true else false, positive = + to account balance
            Transrow["Date"] = dt; // Same Jan 1st yy-julian blank value
            
            //System.Console.WriteLine("Pre-budget check: " + acct.getAcctTbl().Rows.Count + "\n");
            if (acct.getBudget(acct1) == acct.getBudget(acct2)) {
                return -1;
            }
            if (acct.getBudget(acct1) == ((Boolean) true)) {
                Transrow["Budgetacct"] = acct1;
                Transrow["Realacct"] = acct2;
            } else {
                Transrow["Budgetacct"] = acct2;
                Transrow["Realacct"] = acct1;
            }
            //System.Console.WriteLine("Post-budget checks: " + acct.getAcctTbl().Rows.Count + "\n");
            if (setTrans(Transrow))
            {
                TransTable.AcceptChanges();
                //Console.WriteLine(Transrow["transId"].ToString());
                return (Int64) Transrow["transId"];
            } else {
                TransTable.RejectChanges();
                return -1;
            }

        }

        public DataRow getTrans(Int64 TransId = -1) {
            // Returns a new blank row with the correct columns
            return DBase.getRow("transactions",TransId);
        }

        public bool setTrans(DataRow Transaction) {
            // Received a valid budget type row and commits it, if new then add else update
            // Validate table columns match underlying table
            
            // Should do a column validation here but won't for the time being

            // What is the bug here? Above it assigns budgetacct based on the result of getbudget
            // The debugging writeline's are showing true values
            // But if they match up this shouldn't drop in here regardless.
            // Budget account must be true, realaccount must be false
            // If budget false, or realacct true then fail
            if (   (acct.getBudget((Int64) Transaction["Budgetacct"]) == ((Boolean) false))
                || (acct.getBudget((Int64) Transaction["Realacct"]) == ((Boolean) true)))
            {
                return false;
            }
            getTransTbl().LoadDataRow(Transaction.ItemArray, LoadOption.PreserveChanges);
            getTransTbl().AcceptChanges();
            // Update working balance immediately
            // This started failing now. Update account returning no original data to access?
            acct.updateWorkBal((Int64) Transaction["Realacct"], (Double) Transaction["Amount"], (Boolean) Transaction["direction"]);
            return getTransTbl().Rows.Contains(Transaction["transId"]);
        }

        public bool rmTrans(Int64 key = -1) {
            // Stub
            // This should probably be some disable but not delete, or a counter transaction,
            // Generally in accounting one never removes a receipt from the register but reverses it.
            return true;
        }

        // Primarily a testing function
        public int Count() {
            int retVal = 0;
            // Apparently table.rows.count throws an exception when there are no rows.
            try {
                retVal = TransTable.Rows.Count;
                //System.Console.WriteLine(retVal);
            } catch (Exception e) { String useless = e.ToString(); } // Make the compiler warning about not using 'e' go away
            //System.Console.WriteLine(retVal);
            //return retVal;
            return retVal;
        }

        /*
        public double sumTrans(Int64 queryacct, Int64 startDt = 0, Int64 endDt = 0, Boolean cleared = false) {
            // Same song next verse, select is the only thing that changes. Maybe worth moving that out to another bit of code and using these to wrap it and not duplicate out the math.
            Double balance = 0.00;
            // Maybe just update all 3 balances by the cleared/recon flags in
            try {
                //System.Console.WriteLine("Calling transaction lookup with values: " + queryacct + " : " + startDt + " : " + endDt);
                DataRow[] transactionSet = getTransbyAcctDt(queryacct, cleared, startDt, endDt); 
                if (transactionSet != null) { 
                    foreach (DataRow trans in transactionSet) {
                        balance = balance + ((bool) trans["direction"] == true ? Double.Parse(trans["Amount"].ToString()) : (-1 * Double.Parse(trans["amount"].ToString())));
                        //System.Console.WriteLine("transId, Amount: " + trans["transId"].ToString() + " " + trans["Amount"].ToString());
                    } 
                }
                //else { System.Console.WriteLine("Query returned null set with values: " + queryacct); }
            } catch (Exception e) {
                System.Console.WriteLine("Data set query through sumTrans returned a null set which was not pre-checked");
                System.Console.WriteLine("Error: " + e.Message);
            }
            //System.Console.WriteLine("Called transaction lookup with values: " + queryacct + " : " + startDt + " : " + endDt);
            //System.Console.WriteLine("Summation: " + balance);
            return balance;
        }*/

        // Will this ever need the cleared only flag? Would risk accidentally re-counting?
        public double sumTrans(Int64 queryacct, Int64 startDt = 0, Int64 endDt = 0, Boolean cleared = false) {
            // Same song next verse, select is the only thing that changes. Maybe worth moving that out to another bit of code and using these to wrap it and not duplicate out the math.
            Double balance = 0.00;
            Double clrbal = 0.00;
            Double reconbal = 0.00;
            Double amt = 0.00;
            Int64 process = 0;
            bool clradd = false;
            bool reconadd = false;
            if (acct.existsAcct(queryacct) == true ) {
            DataRow Account = acct.getAcct(queryacct);
            } else { return 0.00; }
            // Maybe just update all 3 balances by the cleared/recon flags in one go?
            try {
                //System.Console.WriteLine("Calling transaction lookup with values: " + queryacct + " : " + startDt + " : " + endDt);
                DataRow[] transactionSet = getTransbyAcctDt(queryacct, cleared, startDt, endDt); 
                if (transactionSet != null) { 
                    foreach (DataRow trans in transactionSet) {
                        Int64.TryParse(trans["process"].ToString(), out process);
                        if (   process == 2 || process == 3 
                            || process == 6 || process == 7) { clradd = true; }
                        if (process > 3) { reconadd = true; }
                        Double.TryParse(trans["Amount"].ToString(), out amt);
                        amt = AmttoBal(amt,(bool) trans["direction"]);
                        // Leaving all these comments through the next commit so they exist for later reference in the git repo.
                        // Status codes octal binary flags 1, 2, 4. only invalid code is 0b101
                        // 1 == balance
                        // 2 == clrbal
                        // 3 == balance + clrbal
                        // 4 == reconbal
                        // 5 == reconbal + balance (Should never occur! Never a state where reconbal but not cleared)
                        // 6 == reconbal + clrbal
                        // 7 == reconbal + balance + clrbal 
                        if (process <= 0) { process = 1; trans["process"] = 1; } // Once this is called it will at the least be in the balance
                        // This if could also include process code 5 but that is the invalid one
                        balance = (process == 1 || process == 3 || process == 7) ? balance + amt : balance;
                        // Never pulling from balance, removal gets problematic for history
                        // If cleared is true and clradd is true, do nothing
                        // If cleared is false and clradd is true, remove
                        // If cleared is true and clradd is false, add
                        // False false then do nothing.
                        if ((bool) trans["Cleared"] != clradd) {
                            // Add or remove, if cleared then add, else subtract
                            clrbal = clradd ? clrbal + amt : clrbal - amt;    
                        }
                        if ((bool) trans["Reconciled"] != reconadd) {
                            // Add or remove, if cleared then add, else subtract
                            reconbal = reconadd ? reconbal + amt : reconbal - amt;    
                        }
                        //System.Console.WriteLine("transId, Amount: " + trans["transId"].ToString() + " " + trans["Amount"].ToString());
                    } 
                }
                //else { System.Console.WriteLine("Query returned null set with values: " + queryacct); }
            } catch (Exception e) {
                System.Console.WriteLine("Data set query through sumTrans returned a null set which was not pre-checked");
                System.Console.WriteLine("Error: " + e.Message);
            }
            acct.updateCurBal(queryacct,amt,AmtDir(amt));
            acct.updateWorkBal(queryacct,amt,AmtDir(amt));
            acct.updateReconBal(queryacct,amt,AmtDir(amt));
            System.Console.WriteLine("Called transaction lookup with values: " + queryacct + " : " + startDt + " : " + endDt);
            System.Console.WriteLine("Summation: " + balance);
            return balance;
        }

        // Wrap another way to call sumTrans
        public double sumTrans(Int64 queryacct,Boolean cleared, Int64 startDt = 0, Int64 endDt = 0) {
            return sumTrans(queryacct,startDt,endDt,cleared);
        }

        public DataRow[] getTransbyAcctDt(Int64 queryacct, Boolean cleared, Int64 startDt, Int64 endDt) {
            String Query = "";
            //System.Console.WriteLine("Starting query with values: " + queryacct + " : " + startDt + " : " + endDt);
            DataRow[] transactionSet = null;
            // Do a quick lookup on the acct to figure out if it is going to be real or budget.
            if (acct.getBudget(queryacct) == true) {
                Query = "budgetacct = " + queryacct.ToString();
            } else {
                Query = "realacct = " + queryacct.ToString();
            }
            if (startDt != 0) {
                Query += " and Date >= "  + startDt.ToString();
            }
            if (endDt != 0) {
                Query += " and Date <= "  + endDt.ToString();
            }
            if (cleared == (Boolean) true) {
                Query += " and cleared = true";
            }
            //System.Console.WriteLine(Query);
            transactionSet = getTransTbl().Select(Query);
            return transactionSet;
        }
        public DataRow[] getTransSet() {
            // Returns a new blank row with the correct columns
            return TransTable.Select();
        }

        // Small snag, if set to cleared, and amount has updated then this needs to be removed from the account
        // Maybe a situation where cleared == false and process is 2,3,6,7 then subtract from clramt?
        // Skip the process codes here, the transaction summation can check flag against process, if process code != flag then add or subtract as correct.
        public void setCleared(Int64 transId, Boolean stat = true) {
            DataRow transaction = getTrans(transId);
            transaction["Cleared"] = stat;
            setTrans(transaction);
        }

        public void setReconciled(Int64 transId, Boolean stat = true) {
            DataRow transaction = getTrans(transId);
            if ((bool) transaction["Cleared"] || !(stat)) {
                transaction["Reconciled"] = stat;
                setTrans(transaction);
            }
        }

        // Pair of helper functions, direction isn't strictly necessary 
        private Double AmttoBal(Double amt, Boolean direction) {
            return ((bool) direction == true ? amt : (-1 * amt));
        }

        private Boolean AmtDir(Double amt) {
            return amt < 0 ? true : false;
        }
        // Will want get/sets for all the columns
        // Also a zero out record would be nice for debugging
        // A method for setting a value across multiple records. Maybe a template linq function to set all <field> - <value> on DataRow[].
        // Or maybe just a simple iterate over set.

        // Want a method for getting transactions for a specific account, through a date range, account/unbalanced or unreconciled and combinations of
        // As a set of records,
        // an array of acct,amt duple's
        // A total


    }
}
