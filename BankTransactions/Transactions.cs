using System;
using System.Collections.Generic;
using System.Linq;

namespace BankTransactions
{
    public class User
    {
        public string name { get; }
        private List<Transaction> _outgoing_transactions;
        private List<Transaction> _incoming_transactions;

        public User(string name)
        {
            _outgoing_transactions = new List<Transaction>();
            _incoming_transactions = new List<Transaction>();
            this.name = name;
        }

        public void AddToOutgoing(Transaction transaction)
        {
            _outgoing_transactions.Add(transaction);
        }
        
        public void AddToIncoming(Transaction transaction)
        {
            _incoming_transactions.Add(transaction);
        }

        public Decimal GetBalance()
        {
            var sum = _incoming_transactions
                .Select(item => item.GetAmount())
                .Sum();
            
            sum -= _outgoing_transactions
                .Select(item => item.GetAmount())
                .Sum();

            return sum;
        }
    }
    
    public class Transaction
    {
        private User _sender;
        private User _recipient;
        
        private string _date;
        private string _desc;
        private Decimal _amount;

        public Transaction(string date, User sender, User recipient, string desc, string amount)
        {
            _date = date;
            _sender = sender;
            _recipient = recipient;
            _desc = desc;
            _amount = Decimal.Parse(amount);
        }

        public Decimal GetAmount()
        {
            return _amount;
        }
        
        
    }
}