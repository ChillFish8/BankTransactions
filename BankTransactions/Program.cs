using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic.FileIO;

namespace BankTransactions
{
    class Program
    {
        static void Main(string[] args)
        {
            var manager = new Manager();
            var reader = new CSVParser("./data/Transactions2014.csv");

            foreach (string[] row in reader)
            {
                manager.AddTransaction(row[0], row[1], row[2], row[3], row[4]);
            }
            
            manager.ListAll();
        }
        
    }

    class CSVParser : IEnumerable
    {
        private readonly string[][] _fields;
        public CSVParser(string filePath)
        {
            var fields = new List<string[]>();
            var parser = new TextFieldParser(filePath);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            while (!parser.EndOfData)
            {
                fields.Add(parser.ReadFields());
            }

            _fields = fields.Skip(1).ToArray();
        }
        public IEnumerator GetEnumerator()
        {
            foreach (var field in _fields)
            {
                yield return field;
            }
        }
    }
}