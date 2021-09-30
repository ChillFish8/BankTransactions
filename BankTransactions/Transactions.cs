using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BankTransactions
{
    public class User
    {
        private readonly List<Transaction> _incomingTransactions;
        public List<Transaction> OutgoingTransactions { get; }
        public string Name { get; }

        public User(string name)
        {
            OutgoingTransactions = new List<Transaction>();
            _incomingTransactions = new List<Transaction>();
            Name = name;
        }


        public void AddToOutgoing(Transaction transaction)
        {
            OutgoingTransactions.Add(transaction);
        }

        public void AddToIncoming(Transaction transaction)
        {
            _incomingTransactions.Add(transaction);
        }

        public IEnumerable<Transaction> GetAllTransactions()
        {
            return OutgoingTransactions.Concat(_incomingTransactions);
        }

        public decimal GetBalance()
        {
            var sum = _incomingTransactions
                .Select(item => item.Amount)
                .Sum();

            sum -= OutgoingTransactions
                .Select(item => item.Amount)
                .Sum();

            return sum;
        }
    }

    public class RawTransaction
    {
        public string Date { get; }
        public string FromAccount { get; }
        public string ToAccount { get; }
        public string Narrative { get; }
        public string Amount { get; }

        public RawTransaction(string date, string fromAccount, string toAccount, string narrative, string amount)
        {
            Date = date;
            FromAccount = fromAccount;
            ToAccount = toAccount;
            Narrative = narrative;
            Amount = amount;
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
        public RawTransaction ToRaw()
        {
            return new RawTransaction(
                Date.ToString(CultureInfo.InvariantCulture),
                Sender.Name,
                Recipient.Name,
                Desc,
                Amount.ToString(CultureInfo.InvariantCulture)
                );
        }

        public User Sender { get; }
        public User Recipient { get; }

        public DateTime Date { get; }
        public string Desc { get; }
        public decimal Amount { get; }
    }
    
    
}