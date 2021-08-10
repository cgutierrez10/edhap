using System;
using System.Data;
using System.IO;
using System.IO.Compression;

namespace edhap
{
    // This whole class may not be necessary. 
    public class Reconcile {
        // Handles the transaction reconcilation processing
        // Reads through each transaction
        // Does the bookkeeping for the real acct
        // And the budget acct
        // Updates the acct run balances

        // Start from the resultset and work through accounts
        // Then up to top levels
        // This would allow balancing one account at a time
        // Select dates/account into resultset
        // Inputs would be resultset and runbal - workingbal = expected reconcile?
        
        public Reconcile(Transactions Trans) {
            // Just foreach over all accounts and date to date
            // Grab account and last balance date, then reconcile all transactions since then?
            //foreach (DataRow entry in Trans.getTrans()) {

            //}
        }

    }
}