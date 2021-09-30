using System;
using System.Collections.Generic;
using System.Linq;

namespace BankTransactions
{
    public class User
    {
        private readonly List<Transaction> _incoming_transactions;
        private readonly List<Transaction> _outgoing_transactions;

        public User(string name)
        {
            _outgoing_transactions = new List<Transaction>();
            _incoming_transactions = new List<Transaction>();
            this.name = name;
        }

        public string name { get; }

        public void AddToOutgoing(Transaction transaction)
        {
            _outgoing_transactions.Add(transaction);
        }

        public void AddToIncoming(Transaction transaction)
        {
            _incoming_transactions.Add(transaction);
        }

        public IEnumerable<Transaction> GetAllTransactions()
        {
            return _outgoing_transactions.Concat(_incoming_transactions);
        }

        public decimal GetBalance()
        {
            var sum = _incoming_transactions
                .Select(item => item.amount)
                .Sum();

            sum -= _outgoing_transactions
                .Select(item => item.amount)
                .Sum();

            return sum;
        }
    }

    public class Transaction
    {
        public Transaction(DateTime date, User sender, User recipient, string desc, decimal amount)
        {
            this.date = date;
            this.sender = sender;
            this.recipient = recipient;
            this.desc = desc;
            this.amount = amount;
        }

        public User sender { get; }
        public User recipient { get; }

        public DateTime date { get; }
        public string desc { get; }
        public decimal amount { get; }
    }
}