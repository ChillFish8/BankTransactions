using System;
using Microsoft.Data.Sqlite;

namespace BankTransactions
{
    public class SqliteManager
    {
        private SqliteConnection _connection;

        public SqliteManager()
        {
            _connection = new SqliteConnection(":memory:");

            var cmd = _connection.CreateCommand();
            cmd.CommandText =
                "CREATE TABLE transactions (transaction_date TEXT, sender TEXT, recipient TEXT, description TEXT, amount INTEGER);";
            cmd.ExecuteNonQuery();
        }

        public void AddTransaction(string date, string sender, string recipient, string desc, string amount)
        {
            
        }
    }
}