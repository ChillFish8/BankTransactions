using System;
using System.Collections.Generic;
using System.Linq;

namespace BankTransactions
{
    public class User
    {
        private readonly List<Transaction> _incomingTransactions;
        private readonly List<Transaction> _outgoingTransactions;

        public User(string name)
        {
            _outgoingTransactions = new List<Transaction>();
            _incomingTransactions = new List<Transaction>();
            this.Name = name;
        }

        public string Name { get; }

        public void AddToOutgoing(Transaction transaction)
        {
            _outgoingTransactions.Add(transaction);
        }

        public void AddToIncoming(Transaction transaction)
        {
            _incomingTransactions.Add(transaction);
        }

        public IEnumerable<Transaction> GetAllTransactions()
        {
            return _outgoingTransactions.Concat(_incomingTransactions);
        }

        public decimal GetBalance()
        {
            var sum = _incomingTransactions
                .Select(item => item.Amount)
                .Sum();

            sum -= _outgoingTransactions
                .Select(item => item.Amount)
                .Sum();

            return sum;
        }
    }

    public class Transaction
    {
        public Transaction(DateTime date, User sender, User recipient, string desc, decimal amount)
        {
            Date = date;
            Sender = sender;
            Recipient = recipient;
            Desc = desc;
            Amount = amount;
        }

        public User Sender { get; }
        public User Recipient { get; }

        public DateTime Date { get; }
        public string Desc { get; }
        public decimal Amount { get; }
    }
}