using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Additionneur.Classes.MySqlCrud
{
    public class MySqlManager
    {
        private MySqlConnection Connection;

        private List<Table> Tables = [];

        /// <summary>
        /// MySQL CRUD Manager <br/>
        /// Example connection string:<br/> "Server={server};Database={db};Uid={user};Pwd={pwd};Allow User Variables=True"
        /// </summary>
        /// <param name="connectionString">Ex:<br/> "Server={server};Database={db};Uid={user};Pwd={pwd};Allow User Variables=True"</param>
        public MySqlManager() 
        {

            // Load the XML config file and get the DBCONFIG element to extract the values.
            XDocument xmlDoc = XDocument.Load("./Classes/MySqlCrud/MySqlCrudConfig.xml");
            var xElement = xmlDoc.Descendants("DBCONFIG").First();
            // Initialize the connection string
            string conString = "";

            // Builds the connection string from the XML config file (disables the warning as a correctly configured XML file should always have these values anyway
#pragma warning disable CS8602 
            conString += $"Server={xElement.Element("server").Value};";
            conString += $"Database={xElement.Element("database").Value};";
            conString += $"Uid={xElement.Element("user").Value};";
            conString += $"Pwd={xElement.Element("password").Value};";
            conString += $"{xElement.Element("parameters").Value};";
#pragma warning restore CS8602

            // Removes the \n and \t from the built string
            conString = conString.Replace("\n", "").Replace("\t", "");

            // Creates the Connection
            Connection = new MySqlConnection(conString);
            

            // Gets the list of all the tables names and then use them to add new Table objects to the manager's list
            foreach(string table in GetTablesInit())
            {
                Tables.Add(new Table(conString, table));
            }

            //PrintDbStructure();

        }

        private List<string> GetTablesInit()
        {
            // List of names that'll be used for initialization
            List<string> tables = [];

            // Creates the command
            using (MySqlCommand comm = new MySqlCommand())
            {
                //Opens Connection to DB
                Connection.Open();

                // Setups the command
                comm.CommandText = "SHOW TABLES";
                comm.CommandType = CommandType.Text;
                comm.Connection = Connection;

                // Creates the reader that'll access the data
                MySqlDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {   // Adds each name returned by the query to the list
                    tables.Add(reader.GetString(0));
                }

                // Closes Connection to DB
                Connection.Close();
            }
            // returns the list of tables names
            return tables;

        }

        /// <summary>
        /// Gets a table by table name.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns>Returns a table, or null if none found.</returns>
        public Table GetTable(string tableName)
        {
            foreach(Table table in Tables)
            {
                if(table.Name == tableName) return table;
            }

            return null;
        }

        public void PrintDbStructure()
        {
            foreach (Table table in Tables)
            {
                Trace.WriteLine($" /------{table.Name}-------");
                Dictionary<string, string> columns = table.GetColumns();
                foreach (KeyValuePair<string, string> column in columns)
                {
                    Trace.WriteLine($"| {column.Key} : {column.Value}");
                }
                Trace.WriteLine($" \\________________________");
            }
        }

    }
}
