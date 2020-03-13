using Microsoft.Data.Sqlite;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SqliteIntro
{
    class Program
    {
        static int Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "SqliteIntro",
                FullName = "Sqlite Execute Query",
                Description = "Executes queries from the command line",
            };
            app.HelpOption("-?|-h|--help");

            // SqliteIntro -f "c:\DB\database.sqlite" -q "SELECT * FROM tasks" -o test.csv

              var output = app.Option("-o|--output <FILEPATH>", "Path to save CSV to.", CommandOptionType.SingleValue);
            var query = app.Option("-q|--query <QUERY>", "Sql query to execute", CommandOptionType.SingleValue);
            var file = app.Option("-f|--file <FILEPATH>", "Path and file name of Sqlite database", CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                Console.WriteLine($"Executing query: {query.Value()} from DB: {file.Value()} ");

                if (!File.Exists(file.Value()))
                {
                    return 2; // File not found
                }
                string connectionString = $"Data Source = {file.Value()}";

                var dt = SqliteDB(connectionString, query.Value());
                if (dt.Rows.Count > 0)
                {
                    File.WriteAllText(output.Value(), dt.ToCSV());
                    Console.WriteLine($"CSV file written: {Path.GetFullPath(output.Value())}");
                }
                else
                {
                    Console.WriteLine("Query produced no result.");
                }
                var exitCode = 0; // program executed successfull
                return exitCode;
            });


            try
            {
                return app.Execute(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.ToString());
            }

            return 1; // program failed 
        }

        public static DataTable SqliteDB(string connString, string query)
        {
            DataTable dataTable = new DataTable();

            using (SqliteConnection conn = new SqliteConnection(connString))
            using (SqliteCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = query;
                SqliteDataReader reader = cmd.ExecuteReader();
                dataTable.Load(reader);
            }
            return dataTable;
        }
    }

}


//string connectionString = @"Data Source= \\cmsdev\c$\ProgramData\Advanced Task Scheduler Network\allusers.sqlite";
//string query = "SELECT * FROM tasks";

//var dt = SqliteDB(connectionString, query);


//Console.WriteLine("Hello World!");
//        }