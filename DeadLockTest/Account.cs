using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadLockTest
{
    public class Account
    {
        public int AccountNumber; 
        public decimal Amount; 
        public string ownerName;
    }

    public class TransferHelper
    {
        public void TransferLock(Account a, Account b, decimal amount)
        {
            lock (a)
            {
                Thread.Sleep(5000);
                lock (b)
                {
                    
                    a.Amount -= amount;
                    b.Amount += amount;
                }
            }
        }

        public void TransferSuccess(Account a, Account b, decimal amount)
        {
            var first = a.AccountNumber < b.AccountNumber ? a : b;
            var second = a.AccountNumber < b.AccountNumber ? b : a;

            lock (first)
            {
                Thread.Sleep(5000);
                lock (second)
                {

                    a.Amount -= amount;
                    b.Amount += amount;
                }
            }
        }
    }
}
