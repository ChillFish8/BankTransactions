using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Microsoft.Data.Sqlite;

namespace BankTransactions
{
    public class Manager
    {
        private Dictionary<string, User> _users;
        public Manager()
        {
            _users = new Dictionary<string, User>();
        }

        public void AddTransaction(string date, string sender, string recipient, string desc, string amount)
        {
            var transaction = new Transaction(date, GetOrCreateUser(sender), GetOrCreateUser(recipient), desc, amount);
            _users[sender].AddToOutgoing(transaction);
            _users[recipient].AddToIncoming(transaction);
        }

        private User GetOrCreateUser(string name)
        {
            if (_users.ContainsKey(name))
                return _users[name];
            var user = new User(name);
            _users[name] = user;
            return user;
        }

        public void ListAll()
        {
            if (_users.Count == 0)
            {
                Console.WriteLine("No transactions.");
                return;
            }
            
            var maybePadBy = _users
                .Keys
                .OrderByDescending(item => item.Length)
                .First();

            var padBy = maybePadBy.Length.ToString();
            var formatter = $"{{0,-{padBy}}} -> {{1}}£{{2}}";

            foreach (var (name, user) in _users)
            {
                var balance = user.GetBalance();
                var formattedBalance = Math.Abs(balance).ToString(CultureInfo.InvariantCulture);

                var sign = balance >= 0 ? "+" : "-";
                var line = string.Format(formatter, name, sign, formattedBalance);
                Console.WriteLine(line);
            }
        }

        public void ListAccount(string userName)
        {
            var user = GetOrCreateUser(userName);
            var userTransactions = user.GetAllTransactions().ToList();
            if (userTransactions.Count == 0)
            {
                Console.WriteLine("This user has no transactions.");
                return;
            }

            var padByReceiver = userTransactions
                .Select(item => item.recipient.name)
                .OrderByDescending(item => item.Length)
                .First()
                .Length
                .ToString();
            
            var padBySender = userTransactions
                .Select(item => item.sender.name)
                .OrderByDescending(item => item.Length)
                .First()
                .Length
                .ToString();
            
            var formatter = $"{{0}} ) {{1,-{padBySender}}} -> {{2,-{padByReceiver}}} | £{{3}} -> '{{4}}'";
            
            foreach (var transaction in userTransactions)
            {
                Console.WriteLine(
                    formatter, 
                    transaction.date, 
                    transaction.sender.name, 
                    transaction.recipient.name,
                    transaction.amount.ToString(CultureInfo.CurrentCulture),
                    transaction.desc
                    );
            }
        }
    }
}