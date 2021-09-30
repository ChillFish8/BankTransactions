using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using Newtonsoft.Json;
using NLog;

namespace BankTransactions
{
    public class Manager
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<string, User> _users;

        public Manager()
        {
            _users = new Dictionary<string, User>();
        }

        public void AddTransaction(string date, string sender, string recipient, string desc, string amount)
        {
            try
            {
                DateTime parsedDate;
                try
                {
                    parsedDate = DateTime.Parse(date);
                }
                catch (FormatException)
                {
                    parsedDate = DateTime.FromOADate(int.Parse(date));
                }

                var transaction = new Transaction(
                    parsedDate,
                    GetOrCreateUser(sender),
                    GetOrCreateUser(recipient),
                    desc,
                    decimal.Parse(amount)
                );
                _users[sender].AddToOutgoing(transaction);
                _users[recipient].AddToIncoming(transaction);
            }
            catch (FormatException e)
            {
                Logger.Fatal($"Error adding transaction, invalid transaction data given!\n  {e.Message}");
            }
            catch (Exception e)
            {
                Logger.Fatal($"An unkown exception happened {e}");
            }
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
                .Select(item => item.Recipient.Name)
                .OrderByDescending(item => item.Length)
                .First()
                .Length
                .ToString();

            var padBySender = userTransactions
                .Select(item => item.Sender.Name)
                .OrderByDescending(item => item.Length)
                .First()
                .Length
                .ToString();

            var formatter = $"{{0}} ) {{1,-{padBySender}}} -> {{2,-{padByReceiver}}} | £{{3}} -> '{{4}}'";

            foreach (var transaction in userTransactions)
                Console.WriteLine(
                    formatter,
                    transaction.Date.ToString(CultureInfo.InvariantCulture),
                    transaction.Sender.Name,
                    transaction.Recipient.Name,
                    transaction.Amount.ToString(CultureInfo.InvariantCulture),
                    transaction.Desc
                );
        }

        public void Save(string filePath, string name = "transactions.json")
        {
            var saveItems = new List<RawTransaction>();

            foreach (var user in _users.Values)
            {
                saveItems.AddRange(
                    user.OutgoingTransactions.Select(
                        transaction => transaction.ToRaw()
                        )
                    );
            }

            var data = JsonConvert.SerializeObject(saveItems);
            File.WriteAllText($"{filePath}/{name}", data);
        }
    }
}