using System;
using System.Data;
using System.IO;
using System.IO.Compression;


namespace edhap
{
    class db {
        private DataSet _db = new DataSet(); // Placeholder for coding
        private const String _filename = "temp.sqlite";

        Boolean gzip = false;
        // Provides:
        /*
            ctor blank
            ctor load from file
            Add/remove table from dataset
            create column definition object
            select table if in dataset
            save dataset
        */

        public db() {
            // New blank db, individual tables will have classes that build and impliment access to the table
            // Each one will add itself to this main dataset which provides some helpers like get table, get row, create columns
            // Any new table can be added here initially then moved into a separate class later.
            // Will use the datatable, db from above, db(filename) will open a new one and toss that db. All code will use this db object.
            // Needs to build data tables

            DataTable PayeeTable = new DataTable("payee");
            DataColumn payeeId = newCol("payeeId","Int64");
            payeeId.DefaultValue = null;
            payeeId.AutoIncrement = true;
            PayeeTable.Columns.Add(payeeId);
            DataColumn[] PayeePrimKey = { payeeId };
            PayeeTable.PrimaryKey = PayeePrimKey;
            PayeeTable.Columns.Add(newCol("Payee","String"));
            PayeeTable.Columns.Add(newCol("acctId","Int64")); // Typical budget account this hits, another function can update this over time as the common spending habits change

            _db.Tables.Add(PayeeTable);
        }

        public db(string Filename) {
            // open existing file, reverse of write gzip decompress and readXml
            
            if (gzip) {
                using (var fileStream = File.OpenRead(Filename))
                {        
                    using (var zipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                    {
                        _db.ReadXml(zipStream);
                    } 
                }
            }
            else {
                _db.ReadXml("test.xml");
            }
        }

        public DataColumn newCol(string name, string type) {
            DataColumn Col = new DataColumn();
            Col.DataType = System.Type.GetType("System." + type);
            Col.ColumnName = name;
            if (type == "String") { Col.DefaultValue = ""; }
            else if (type == "Int64") { Col.DefaultValue = -1; }
            else if (type == "Boolean") { Col.DefaultValue = false; }
            else if (type == "Double") { Col.DefaultValue = 0.00; }
            return Col;
        }

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

        public void dropTbl(String tblName) {
             if (_db.Tables.Contains(tblName)) {
                _db.Tables.Remove(tblName);
            }
        }
        public Boolean setTbl(DataTable tbl) {
            // Generic version of get row, table names are always strings and at present keyvals are always ints
            _db.Tables.Add(tbl);
            return true;
        }

        public Boolean saveDb(String writeFile = _filename) {
            if (gzip) {
                using (var fileStream = File.Create(writeFile))
                {   
                    using (var zipStream = new GZipStream(fileStream, CompressionMode.Compress))
                    {
                        _db.WriteXml(zipStream, XmlWriteMode.WriteSchema);
                    } 
                }
            }    
            else {
                _db.WriteXml("test.xml", XmlWriteMode.WriteSchema);
            }   
            return true;
        }
    }
}