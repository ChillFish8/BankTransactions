using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic.FileIO;

namespace BankTransactions
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
        
    }

    class CSVParser : IEnumerable
    {
        private readonly List<string[]> _fields;
        CSVParser(string filePath)
        {
            _fields = new List<string[]>();
            var parser = new TextFieldParser(filePath);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            while (!parser.EndOfData)
            {
                _fields.Add(parser.ReadFields());
            }
        }
        public IEnumerator GetEnumerator()
        {
            foreach (var field in _fields)
            {
                yield return Tuple.Create(field[0], field[1], field[2], field[3], field[4]);
            }
        }
    }
}