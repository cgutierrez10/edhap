/* Budget object finally! */
using System;
using System.Data;
using System.IO;
using System.IO.Compression;

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
        
    }
}