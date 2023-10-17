// -----------------------------------------------------------
// Copyright(c) Coalition of the Good-Hearted Engineers
// ======= FREE TO USE FOR THE WORLD =======
// -----------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SqlQueryProtect
{
    internal class Program
    {

        private const string selectCities = 
            "SELECT Distinct Top(100) CityName FROM [WideWorldImporters].[Application].[Cities]; DROP TABLE City";

        private const string selectCity = "SELECT * FROM [WideWorldImporters].[Application].[City]";
        
        private const string createCity = 
            "CREATE Table [WideWorldImporters].[Application].[City] (CityId INT PRIMARY KEY, CityName varchar(50))";
        
        private const string dropCity = "DROP Table [WideWorldImporters].[Application].[City]";
        private const string attackQuery = "SELECT * FROM myTable WHERE id = {0}, 1 OR 1==1 --";

        static void Main(string[] args)
        {
            var db = new Database();
            ParseQuery(db, attackQuery);
        }

        private static void ParseQuery(Database db, string tsqQuery)
        {
            var rdr = new StringReader(tsqQuery);
            IList<ParseError> errors = null;
            var parser = new TSql150Parser(true, SqlEngineType.All);
            TSqlFragment parseTree = parser.Parse(rdr, out errors);
            MyVisitor checker = new MyVisitor();
            parseTree.Accept(checker);

            foreach (ParseError error in errors)
            {
                Console.WriteLine(error.Message);
            }

            if (checker.containsOnlySelects is true)
            {
                ExecuteQuery(db, tsqQuery);
            }
            else
            {
                
                Console.WriteLine($@"Query validation exception error occured, please correct.");
            }
        }

        class MyVisitor : TSqlFragmentVisitor
        {
            internal bool containsOnlySelects = true;
            public override void Visit(TSqlStatement node)
            {
                if ((node as SelectStatement) is null)
                {
                    containsOnlySelects = false;
                }
                base.Visit(node);
            }
        }
        private static void ExecuteQuery(Database db, string parsedQuery)
        {
            string query = $"{parsedQuery}";

            var executionResult = db.ExecuteQuery(query);

            var jsonString = JsonConvert.SerializeObject(
                executionResult,
                Formatting.Indented,
                new JsonConverter[] { new StringEnumConverter() });

            Console.WriteLine(jsonString);
        }
    }
}