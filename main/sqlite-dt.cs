using System;
using System.Data;
using System.IO;
using System.IO.Compression;


namespace edhap
{
    class db {
        private DataSet _db = new DataSet(); // Placeholder for coding
        private const String _filename = "temp.sqlite";
        // Provides:
        /*
            Add/Change budget line
            Add/Change transaction
            Delete? Budget line, transaction? Should these be deletable or should they be retained and hidden?
            Budget may lose history consistency with deletions
            Add/Change payee
            Retrieve payee budget type guess
            Retrieve account top line dataset (other classes will handle this dataset for doing calculations)
            Save or save as (copy)
        */

        public db() {
            // New blank db
            // Will use the datatable, db from above, db(filename) will open a new one and toss that db. All code will use this db object.
            // Needs to build data tables
            // Column name convention, lcase start means internal, ucase start means will be displayed and visible directly to user
            /*
            DataTable AcctTable = new DataTable("accounts");
            DataColumn acctId = newCol("acctId","Int64");
            acctId.AutoIncrement = true;
            AcctTable.Columns.Add(acctId);
            DataColumn[] AcctPrimKey = { acctId } ;
            AcctTable.PrimaryKey = AcctPrimKey;
            AcctTable.Columns.Add(newCol("Name","String"));
            AcctTable.Columns.Add(newCol("budget","Boolean")); // Boolean, budget or real acct
            AcctTable.Columns.Add(newCol("Tracking","Boolean"));
            AcctTable.Columns.Add(newCol("Balance","Double"));
            AcctTable.Columns.Add(newCol("WorkingBal","Double"));
            AcctTable.Columns.Add(newCol("parent","Int64")); // Ties to another 'account' type object.
            AcctTable.Columns.Add(newCol("Comment","String"));
            AcctTable.Columns.Add(newCol("Carryover","Double"));
            AcctTable.Columns.Add(newCol("LastUpdate","Int64")); // yy-julian date  indicating last time this account was processed, for catch up purposes.
            */

            /*
            DataTable TransTable = new DataTable("transaction");
            DataColumn transId = newCol("transId","Int64");
            transId.AutoIncrement = true;
            TransTable.Columns.Add(transId);
            DataColumn[] TransPrimKey = { transId };
            TransTable.PrimaryKey = TransPrimKey;
            TransTable.Columns.Add(newCol("payeeId","Int64")); // Payee will be id keyed, display will pull the correct id-text keying
            TransTable.Columns.Add(newCol("Amount","Double"));
            TransTable.Columns.Add(newCol("Direction","Boolean"));
            TransTable.Columns.Add(newCol("Cleared","Boolean"));
            TransTable.Columns.Add(newCol("Reconciled","Boolean"));
            TransTable.Columns.Add(newCol("Memo","String"));
            TransTable.Columns.Add(newCol("Date","Int64")); // Same as yy-julan date to be used above
            TransTable.Columns.Add(newCol("Checknum","String"));
            TransTable.Columns.Add(newCol("Realacct","Int64"));
            TransTable.Columns.Add(newCol("Budgetacct","Int64"));
            TransTable.Columns.Add(newCol("splitkey","Int64")); // splitkey is the parent of split transactions
            //TransTable.Columns.Add(newCol("trans-id","Int64")); 
            */

            DataTable PayeeTable = new DataTable("payee");
            DataColumn payeeId = newCol("payeeId","Int64");
            payeeId.AutoIncrement = true;
            PayeeTable.Columns.Add(payeeId);
            DataColumn[] PayeePrimKey = { payeeId };
            PayeeTable.PrimaryKey = PayeePrimKey;
            PayeeTable.Columns.Add(newCol("Payee","String"));
            PayeeTable.Columns.Add(newCol("acctId","Int64")); // Typical budget account this hits, another function can update this over time as the common spending habits change


            //_db.Tables.Add(AcctTable);
            //_db.Tables.Add(TransTable);
            _db.Tables.Add(PayeeTable);

            //saveDb("test.db");
        }

        public db(string Filename) {
            // open existing file, reverse of write gzip decompress and readXml
            using (var fileStream = File.OpenRead(Filename))
            {
                using (var zipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                {
                    _db.ReadXml(zipStream);
                }
            }       
        }

        public DataColumn newCol(string name, string type) {
            DataColumn Col = new DataColumn();
            Col.DataType = System.Type.GetType("System." + type);
            Col.ColumnName = name;
            return Col;
        }



/*        public DataRow getTrans(Int64 TransId = -1) {
            // Returns a new blank transaction with the correct columns
            return getRow("transactions",TransId);
        }*/

        public DataRow getRow(string tblName, Int64 keyVal) {
            // Generic version of get row, table names are always strings and at present keyvals are always ints
            DataTable accts = _db.Tables[tblName];
            DataRow row;
            if (_db.Tables[tblName].Rows.Contains(keyVal)) {
                row = _db.Tables[tblName].Rows.Find(keyVal);
            } else {
                row = _db.Tables[tblName].NewRow();
            }
            return row;
        }

        public DataTable getTbl(string tblName) {
            // Generic version of get row, table names are always strings and at present keyvals are always ints
            DataTable retTbl = null;
            if (_db.Tables.Contains(tblName)) {
                retTbl = _db.Tables[tblName];
            }
            return retTbl;
        }

        public Boolean setTbl(DataTable tbl) {
            // Generic version of get row, table names are always strings and at present keyvals are always ints
            _db.Tables.Add(tbl);
            return true;
        }

/*        public Boolean setTrans(DataRow Transaction) {
            // Received a valid budget type transaction and commits it, if new then add else update
            // Handles single transactions, split transactions will pre-process and create multiple new/updates
            
            
            // Should do a column validation here but won't for the time being
            _db.Tables["transactions"].Rows.Add(Transaction);
            return true;
        }*/

    /*    public Boolean rmTrans(Int64 key = -1) {
            // Stub
            return true;
        }*/

        public Boolean saveDb(String writeFile = _filename) {
            using (var fileStream = File.Create(writeFile))
            {
                using (var zipStream = new GZipStream(fileStream, CompressionMode.Compress))
                {
                    _db.WriteXml(zipStream, XmlWriteMode.WriteSchema);
                }
            }       
            return true;
        }
    }
}