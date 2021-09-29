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
                "CREATE TABLE IF NOT EXISTS transactions "+
                "(transaction_date TEXT, sender TEXT, recipient TEXT, description TEXT, amount DECIMAL);";
            cmd.ExecuteNonQuery();
        }

        public void AddTransaction(string date, string sender, string recipient, string desc, string amount)
        {
            const string stmt =
                "INSERT INTO transactions (transaction_date, sender, recipient, description, amount)" +
                "VALUES(@transaction_date, @sender, @recipient, @description, @amount)";

            var cmd = _connection.CreateCommand();
            cmd.CommandText = stmt;
            cmd.Parameters.AddWithValue("@transaction_date", date);
            cmd.Parameters.AddWithValue("@sender", sender);
            cmd.Parameters.AddWithValue("@recipient", recipient);
            cmd.Parameters.AddWithValue("@description", desc);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Prepare();

            cmd.ExecuteNonQuery();
        }
    }
}