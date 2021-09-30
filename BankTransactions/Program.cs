using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace BankTransactions
{
    class Program
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {

            var config = new LoggingConfiguration();
            var target = new ColoredConsoleTarget();
            config.AddTarget("Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;
            
            Logger.Info("Starting program");
            
            var manager = new Manager();
            var reader = new CSVParser("./data/DodgyTransactions2015.csv");

            foreach (string[] row in reader)
            {
                manager.AddTransaction(row[0], row[1], row[2], row[3], row[4]);
            }
            Logger.Info("Completed all transactions");

            manager.ListAll();
            manager.ListAccount("Sarah T");
            Logger.Info("Program complete :)");

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

    class JSONParser
    {
        public JSONParser(string filePath)
        {
            dynamic fields = JsonConvert.DeserializeObject("");

        }
    }
}