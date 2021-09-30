using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace BankTransactions
{
    internal class Program
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private static void Main(string[] args)
        {
            var config = new LoggingConfiguration();
            var target = new ColoredConsoleTarget();
            config.AddTarget("Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;

            Logger.Info("Starting program");

            var manager = new Manager();

            // var reader = new CSVParser("./data/DodgyTransactions2015.csv");
            // var reader = new JSONParser("./data/Transactions2013.json");
            var reader = new XMLParser("./data/Transactions2012.xml");
            foreach (string[] row in reader) manager.AddTransaction(row[0], row[1], row[2], row[3], row[4]);
            Logger.Info("Completed all transactions");

            manager.ListAll();
            manager.ListAccount("Sarah T");
            Logger.Info("Program complete :)");
        }
    }

    internal class CSVParser : IEnumerable
    {
        private readonly string[][] _fields;

        public CSVParser(string filePath)
        {
            var fields = new List<string[]>();
            var parser = new TextFieldParser(filePath);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            while (!parser.EndOfData) fields.Add(parser.ReadFields());

            _fields = fields.Skip(1).ToArray();
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var field in _fields) yield return field;
        }
    }

    internal class JSONParser : IEnumerable
    {
        private readonly List<dynamic> _fields;

        public JSONParser(string filePath)
        {
            var reader = new StreamReader(filePath);
            var json = reader.ReadToEnd();
            _fields = JsonConvert.DeserializeObject<List<dynamic>>(json);
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var field in _fields)
            {
                string[] value =
                {
                    field.Date.ToString(),
                    field.FromAccount.ToString(),
                    field.ToAccount.ToString(),
                    field.Narrative.ToString(),
                    field.Amount.ToString()
                };
                yield return value;
            }
        }
    }

    internal class XMLParser : IEnumerable
    {
        private readonly XmlDocument fields;

        public XMLParser(string filePath)
        {
            fields = new XmlDocument();
            fields.Load(filePath);
        }

        public IEnumerator GetEnumerator()
        {
            var parent = fields.DocumentElement.ChildNodes;
            foreach (XmlNode row in parent)
            {
                string[] value =
                {
                    row.Attributes["Date"].Value,
                    row.ChildNodes[2].ChildNodes[0].InnerText,
                    row.ChildNodes[2].ChildNodes[1].InnerText,
                    row.ChildNodes[0].InnerText,
                    row.ChildNodes[1].InnerText
                };
                yield return value;
            }
        }
    }
}