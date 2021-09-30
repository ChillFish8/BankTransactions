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
            var reader = new XmlParser("./data/Transactions2012.xml");
            foreach (string[] row in reader) manager.AddTransaction(row[0], row[1], row[2], row[3], row[4]);
            Logger.Info("Completed all transactions");

            manager.ListAll();
            manager.ListAccount("Sarah T");
            manager.Save("./data");
            Logger.Info("Program complete :)");
        }
    }

    internal class CsvParser : IEnumerable
    {
        private readonly string[][] _fields;

        public CsvParser(string filePath)
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

    internal class JsonParser : IEnumerable
    {
        private readonly List<dynamic> _fields;

        public JsonParser(string filePath)
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

    internal class XmlParser : IEnumerable
    {
        private readonly XmlDocument _fields;

        public XmlParser(string filePath)
        {
            _fields = new XmlDocument();
            _fields.Load(filePath);
        }

        public IEnumerator GetEnumerator()
        {
            var parent = _fields.DocumentElement?.ChildNodes;
            if (parent is null)
                yield break;

            foreach (XmlNode row in parent)
            {
                if (row is null)
                    continue;

                string[] value =
                {
                    row.Attributes?["Date"]?.Value,
                    row.ChildNodes[2]?.ChildNodes[0]?.InnerText,
                    row.ChildNodes[2]?.ChildNodes[1]?.InnerText,
                    row.ChildNodes[0]?.InnerText,
                    row.ChildNodes[1]?.InnerText
                };
                yield return value;
            }
        }
    }
}