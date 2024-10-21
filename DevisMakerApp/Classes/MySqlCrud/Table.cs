using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DevisMakerApp.Classes.MySqlCrud
{
    public class Table
    {
        public string Name { get; }

        private MySqlConnection Connection;

        public List<Column> Columns { get; } = [] ;

        public Table(string connectionString, string _name) 
        {
            Name = _name;

            // Creates this table's connection to the DB and opens it
            Connection = new MySqlConnection(connectionString);

            // Gets a dict corresponding to the columns in this table and fills the list of Column of this object with new corresponding column objects
            foreach(KeyValuePair<string, string> Pair in GetColumnsInit())
            {
                Columns.Add(new Column(connectionString, Pair.Key, Pair.Value, this));
            }
        }
        /// <summary>
        /// Used in object initialization, shouldn't be used outside of that.\n
        /// Gets the names and types of column in this table from the database directly.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetColumnsInit()
        {
            // Dictionnary of columns in the table, with {"Column Name", "Column Type"}, which will be used in the initialization.
            Dictionary<string, string> columns = [];

            Connection.Open();

            //Creates the command
            using (MySqlCommand comm = new MySqlCommand())
            {   // Setups the command
                comm.CommandText = $"SHOW COLUMNS FROM {Name}";
                comm.CommandType = CommandType.Text;
                comm.Connection = Connection;

                // Creates the reader that'll access the data
                MySqlDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {   // Adds each name and type returned by the query to the dict
                    columns.Add(reader.GetString(0), reader.GetString(1));
                }
            }

            Connection.Close();

            // Returns the dict of the columns
            return columns;
        }

        // =====[ UTILITARIES ]=====
        /// <summary>
        /// Return a <see cref="Dictionary{TKey, TValue}"/>(string, string) representing the columns in this table, with ("column name","column type").
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetColumns() 
        {
            Dictionary<string, string> columns = [];

            foreach(Column col in Columns)
            {
                columns.Add(col.Name, col.Type);
            }

            return columns;
        }

        /// <summary>
        /// Gets a list containing this table's columns names.
        /// </summary>
        /// <returns></returns>
        public List<string> GetColumnsNames()
        {
            List<string> columnsNames = [];

            foreach (Column col in Columns)
            {
                columnsNames.Add(col.Name);
            }

            return columnsNames;
        }

        /// <summary>
        /// Gets a <see cref="Column"/> by column name.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns>Returns a <see cref="Column"/>, or null if none found.</returns>
        public Column GetColumn(string columnName)
        {
            foreach (Column col in Columns)
            {
                if (col.Name == columnName) return col;
            }

            return null;

        }



        // =====[ INSERT ]=====
        /// <summary>
        /// Inserts a new row in the table.
        /// Takes a <see cref="Dictionary{TKey, TValue}"/> with {columnName, columnValue}.
        /// </summary>
        /// <param name="values"></param>
        /// <exception cref="Exception"></exception>
        public void InsertRow(Dictionary<string, object> values)
        {
            // Get names of column to verify that they exist later
            List<string> columnNames = GetColumnsNames();

            // Strings that'll be used in the command, first for the column names and second for the corresponding values
            string reqColumns = "(";
            string reqValues = "(";

            //Iterates through the values dict
            foreach (KeyValuePair<string, object> pair in values)
            {
                // Verifies that the column exists
                if(!columnNames.Contains(pair.Key))
                {
                    // Throws exception if it doesn't
                    throw new Exception($"MySqlManager.Table Exception: Tried to put value in {pair.Key} column, which does not exist in {Name} table.");
                }

                // if it does, adds the column to the column string and the column name with an @ to the values string to pass it as a parameter later on
                reqColumns += pair.Key + ",";
                reqValues += $"@{pair.Key},";


            }

            // trims the excess comma and closes the parenthesis
            reqColumns = reqColumns.TrimEnd(',') + ")";
            reqValues = reqValues.TrimEnd(',') + ")";

            // builds the final query
            string req = $"INSERT INTO {Name}{reqColumns} VALUES{reqValues}";

            Trace.WriteLine(req);

            try
            {
                // Creates the command object
                using(MySqlCommand comm = new())
                {
                    // Opens the connection to the DB
                    Connection.Open();

                    // Setups the command
                    comm.CommandText = req;
                    comm.CommandType = CommandType.Text;
                    comm.Connection = Connection;

                    // Adds the attributes for each column name (@columnName => columnValue)
                    foreach(KeyValuePair<string, object> pair in values)
                    {
                        comm.Parameters.Add(new MySqlParameter($"@{pair.Key}", pair.Value));
                    }

                    // Executes the command
                    comm.ExecuteNonQuery();

                    // Closes the connection
                    Connection.Close();
                }
            }
            catch (Exception e)
            {
                // Writes exception if caught
                Trace.WriteLine(e);
            }


        }

        public void InsertRows(Dictionary<string, object[]> values)
        {
            // Get names of column to verify that they exist later
            List<string> columnNames = GetColumnsNames();

            // Strings that'll be used in the command, first for the column names and second for the corresponding values
            string reqColumns = "(";

            int valuesAmount = values.ElementAt(0).Value.Length;

            string[] reqValues = new string[valuesAmount];

            for(int i = 0; i < valuesAmount; i++)
            {
                reqValues[i] = "(";
            }

            //Iterates through the values dict
            foreach (KeyValuePair<string, object[]> pair in values)
            {
                // Verifies that the column exists
                if (!columnNames.Contains(pair.Key))
                {
                    // Throws exception if it doesn't
                    throw new Exception($"MySqlManager.Table Exception: Tried to put value in {pair.Key} column, which does not exist in {Name} table.");
                }

                // if it does, adds the column to the column string and the column name with an @ to the values string to pass it as a parameter later on
                reqColumns += pair.Key + ",";

                for(int i = 0; i < pair.Value.Length; i++)
                {
                    reqValues[i] += $"@{pair.Key}{i},";
                }
            }

            // trims the excess comma and closes the parenthesis
            reqColumns = reqColumns.TrimEnd(',') + ")";
            for (int i = 0; i < valuesAmount; i++)
            {
                reqValues[i] = reqValues[i].TrimEnd(',') + ")";
            }
            

            // builds the final query
            string req = $"INSERT INTO {Name}{reqColumns} VALUES ";

            foreach(string s in  reqValues)
            {
                req += s + ",";
            }
            req = req.TrimEnd(',');

            Trace.WriteLine(req);

            try
            {
                // Creates the command object
                using (MySqlCommand comm = new())
                {
                    // Opens the connection to the DB
                    Connection.Open();

                    // Setups the command
                    comm.CommandText = req;
                    comm.CommandType = CommandType.Text;
                    comm.Connection = Connection;

                    // Adds the attributes for each column name (@columnName => columnValue)
                    foreach (KeyValuePair<string, object[]> pair in values)
                    {
                        for (int i = 0; i < valuesAmount; i++)
                        {
                            comm.Parameters.Add(new MySqlParameter($"@{pair.Key}{i}", pair.Value[i]));
                        }
                    }

                    // Executes the command
                    comm.ExecuteNonQuery();

                    // Closes the connection
                    Connection.Close();
                }
            }
            catch (Exception e)
            {
                // Writes exception if caught
                Trace.WriteLine(e);
            }


        }

        // =====[ UPDATE ]=====
        /// <summary>
        /// Updates a single row, takes a <see cref="Dictionary{TKey, TValue}"/>(string, object), with ("column name", new value),<br/> the name and value of the column to check (column = value condition)
        /// </summary>
        /// <param name="newValues">A <see cref="Dictionary{TKey, TValue}"/>(string, object), with ("column name", new value)</param>
        /// <param name="columnToCheck">A <see cref="string"/>, name of the column on which to test the value.</param>
        /// <param name="valueToCheck">An <see cref="object"/>, value to check in the column.</param>
        /// <exception cref="Exception"></exception>
        public void UpdateRow(Dictionary<string, object> newValues, string columnToCheck, object valueToCheck)
        {
            // Get names of column to verify that they exist later
            List<string> columnNames = GetColumnsNames();

            // String that'll be used in the command for setting the new values
            string reqSet = "SET ";

            //Iterates through the values dict
            foreach (KeyValuePair<string, object> pair in newValues)
            {
                // Verifies that the column exists
                if (!columnNames.Contains(pair.Key))
                {
                    // Throws exception if it doesn't
                    throw new Exception($"MySqlManager.Table Exception: Tried to update value in {pair.Key} column, which does not exist in {Name} table. \n (Columns in this table are: {columnNames})");
                }

                // if it does, adds to the SET string with columnName = @columnName with an @ to pass the value as a parameter later on
                reqSet += $"{pair.Key} = @{pair.Key},";
            }
            // Removes the excess comma at the end
            reqSet = reqSet.TrimEnd(',');

            // Builds the query
            string req = $"UPDATE {Name} {reqSet} WHERE {columnToCheck} = {valueToCheck} LIMIT 1";

            try
            {
                // Creates the command object
                using (MySqlCommand comm = new())
                {
                    // Opens the connection to the DB
                    Connection.Open();

                    // Setups the command
                    comm.CommandText = req;
                    comm.CommandType = CommandType.Text;
                    comm.Connection = Connection;

                    // Adds the attributes for each column name (@columnName => columnValue)
                    foreach (KeyValuePair<string, object> pair in newValues)
                    {
                        comm.Parameters.Add(new MySqlParameter($"@{pair.Key}", pair.Value));
                        Trace.WriteLine(pair.Key + ": " + pair.Value);
                    }

                    // Executes the command
                    comm.ExecuteNonQuery();

                    // Closes the connection
                    Connection.Close();
                }
            }
            catch (Exception e)
            {
                // Writes exception if caught
                Trace.WriteLine(e);
            }

        }
        /// <summary>
        /// Updates a single row, takes a <see cref="Dictionary{TKey, TValue}"/>(string, object), with ("column name", new value),<br/> the names and values of the columns to check (column = value conditions)
        /// </summary>
        /// <param name="newValues"></param>
        /// <param name="columnsToCheck"></param>
        /// <param name="valuesToCheck"></param>
        /// <exception cref="Exception"></exception>
        public void UpdateRow(Dictionary<string, object> newValues, string[] columnsToCheck, object[] valuesToCheck)
        {
            // Get names of column to verify that they exist later
            List<string> columnNames = GetColumnsNames();

            // String that'll be used in the command for setting the new values
            string reqSet = "SET ";

            //Iterates through the values dict
            foreach (KeyValuePair<string, object> pair in newValues)
            {
                // Verifies that the column exists
                if (!columnNames.Contains(pair.Key))
                {
                    // Throws exception if it doesn't
                    throw new Exception($"MySqlManager.Table Exception: Tried to update value in {pair.Key} column, which does not exist in {Name} table. \n (Columns in this table are: {columnNames})");
                }

                // if it does, adds to the SET string with columnName = @columnName with an @ to pass the value as a parameter later on
                reqSet += $"{pair.Key} = @{pair.Key},";
            }
            // Removes the excess comma at the end
            reqSet = reqSet.TrimEnd(',');

            // Builds the query
            string req = $"UPDATE {Name} {reqSet} WHERE ";

            // Iterates through the parameters to add their condition to the query
            for (int i = 0; i < columnsToCheck.Length; i++)
            {
                req += $"{columnsToCheck[i]} = {valuesToCheck[i]} AND ";
            }
            // Removes the last 5 chars, corresponding to the excess " AND ", then add the limit
            req = req.Remove(req.Length - 5) + " LIMIT 1";

            try
            {
                // Creates the command object
                using (MySqlCommand comm = new())
                {
                    // Opens the connection to the DB
                    Connection.Open();

                    // Setups the command
                    comm.CommandText = req;
                    comm.CommandType = CommandType.Text;
                    comm.Connection = Connection;

                    // Adds the attributes for each column name (@columnName => columnValue)
                    foreach (KeyValuePair<string, object> pair in newValues)
                    {
                        comm.Parameters.Add(new MySqlParameter($"@{pair.Key}", pair.Value));
                        Trace.WriteLine(pair.Key + ": " + pair.Value);
                    }

                    // Executes the command
                    comm.ExecuteNonQuery();

                    // Closes the connection
                    Connection.Close();
                }
            }
            catch (Exception e)
            {
                // Writes exception if caught
                Trace.WriteLine(e);
            }

        }

        /// <summary>
        /// Updates rows, takes a <see cref="Dictionary{TKey, TValue}"/>(string, object), with ("column name", new value),<br/> the name and value of the column to check (column = value condition)
        /// </summary>
        /// <param name="newValues">A <see cref="Dictionary{TKey, TValue}"/>(string, object), with ("column name", new value)</param>
        /// <param name="columnToCheck">A <see cref="string"/>, name of the column on which to test the value.</param>
        /// <param name="valueToCheck">An <see cref="object"/>, value to check in the column.</param>
        /// <exception cref="Exception"></exception>
        public void UpdateRows(Dictionary<string, object> newValues, string columnToCheck, object valueToCheck)
        {
            // Get names of column to verify that they exist later
            List<string> columnNames = GetColumnsNames();

            // String that'll be used in the command for setting the new values
            string reqSet = "SET ";

            //Iterates through the values dict
            foreach (KeyValuePair<string, object> pair in newValues)
            {
                // Verifies that the column exists
                if (!columnNames.Contains(pair.Key))
                {
                    // Throws exception if it doesn't
                    throw new Exception($"MySqlManager.Table Exception: Tried to update value in {pair.Key} column, which does not exist in {Name} table. \n (Columns in this table are: {columnNames})");
                }

                // if it does, adds to the SET string with columnName = @columnName with an @ to pass the value as a parameter later on
                reqSet += $"{pair.Key} = @{pair.Key},";
            }
            // Removes the excess comma at the end
            reqSet = reqSet.TrimEnd(',');

            // Builds the query
            string req = $"UPDATE {Name} {reqSet} WHERE {columnToCheck} = {valueToCheck}";

            try
            {
                // Creates the command object
                using (MySqlCommand comm = new())
                {
                    // Opens the connection to the DB
                    Connection.Open();

                    // Setups the command
                    comm.CommandText = req;
                    comm.CommandType = CommandType.Text;
                    comm.Connection = Connection;

                    // Adds the attributes for each column name (@columnName => columnValue)
                    foreach (KeyValuePair<string, object> pair in newValues)
                    {
                        comm.Parameters.Add(new MySqlParameter($"@{pair.Key}", pair.Value));
                        Trace.WriteLine(pair.Key + ": " + pair.Value);
                    }

                    // Executes the command
                    comm.ExecuteNonQuery();

                    // Closes the connection
                    Connection.Close();
                }
            }
            catch (Exception e)
            {
                // Writes exception if caught
                Trace.WriteLine(e);
            }

        }
        /// <summary>
        /// Updates rows, takes a <see cref="Dictionary{TKey, TValue}"/>(string, object), with ("column name", new value),<br/> the names and values of the columns to check (column = value conditions)
        /// </summary>
        /// <param name="newValues"></param>
        /// <param name="columnsToCheck"></param>
        /// <param name="valuesToCheck"></param>
        /// <exception cref="Exception"></exception>
        public void UpdateRows(Dictionary<string, object> newValues, string[] columnsToCheck, object[] valuesToCheck)
        {
            // Get names of column to verify that they exist later
            List<string> columnNames = GetColumnsNames();

            // String that'll be used in the command for setting the new values
            string reqSet = "SET ";

            //Iterates through the values dict
            foreach (KeyValuePair<string, object> pair in newValues)
            {
                // Verifies that the column exists
                if (!columnNames.Contains(pair.Key))
                {
                    // Throws exception if it doesn't
                    throw new Exception($"MySqlManager.Table Exception: Tried to update value in {pair.Key} column, which does not exist in {Name} table. \n (Columns in this table are: {columnNames})");
                }

                // if it does, adds to the SET string with columnName = @columnName with an @ to pass the value as a parameter later on
                reqSet += $"{pair.Key} = @{pair.Key},";
            }
            // Removes the excess comma at the end
            reqSet = reqSet.TrimEnd(',');

            // Builds the query
            string req = $"UPDATE {Name} {reqSet} WHERE ";

            // Iterates through the parameters to add their condition to the query
            for (int i = 0; i < columnsToCheck.Length; i++)
            {
                req += $"{columnsToCheck[i]} = {valuesToCheck[i]} AND ";
            }
            // Removes the last 5 chars, corresponding to the excess " AND ", then add the limit
            req = req.Remove(req.Length - 5);

            try
            {
                // Creates the command object
                using (MySqlCommand comm = new())
                {
                    // Opens the connection to the DB
                    Connection.Open();

                    // Setups the command
                    comm.CommandText = req;
                    comm.CommandType = CommandType.Text;
                    comm.Connection = Connection;

                    // Adds the attributes for each column name (@columnName => columnValue)
                    foreach (KeyValuePair<string, object> pair in newValues)
                    {
                        comm.Parameters.Add(new MySqlParameter($"@{pair.Key}", pair.Value));
                        Trace.WriteLine(pair.Key + ": " + pair.Value);
                    }

                    // Executes the command
                    comm.ExecuteNonQuery();

                    // Closes the connection
                    Connection.Close();
                }
            }
            catch (Exception e)
            {
                // Writes exception if caught
                Trace.WriteLine(e);
            }

        }

        // =====[ DELETE ]=====
        /// <summary>
        /// Delete a single row (first encountered if more than one present). Based on a = condition.
        /// </summary>
        /// <param name="columnToCheck"></param>
        /// <param name="valueToCheck"></param>
        public void DeleteRow(string columnToCheck, object valueToCheck)
        {
            // Builds the query
            string req = $"DELETE FROM {Name} WHERE {columnToCheck} = {valueToCheck} LIMIT 1";

            try
            {
                // Creates the command object
                using (MySqlCommand comm = new())
                {
                    // Opens the connection to the DB
                    Connection.Open();

                    // Setups the command
                    comm.CommandText = req;
                    comm.CommandType = CommandType.Text;
                    comm.Connection = Connection;

                    // Executes the command
                    comm.ExecuteNonQuery();

                    // Closes the connection
                    Connection.Close();
                }
            }
            catch (Exception e)
            {
                // Writes exception if caught
                Trace.WriteLine(e);
            }
        }

        /// <summary>
        /// Delete a single row (first encountered if more than one present). Based on multiple = condition.
        /// </summary>
        /// <param name="columnToCheck"></param>
        /// <param name="valueToCheck"></param>
        public void DeleteRow(string[] columnsToCheck, object[] valuesToCheck)
        {
            // Builds the query
            string req = $"DELETE FROM {Name} WHERE ";

            // Iterates through the parameters to add their condition to the query
            for (int i = 0; i < columnsToCheck.Length; i++)
            {
                req += $"{columnsToCheck[i]} = {valuesToCheck[i]} AND ";
            }
            // Removes the last 5 chars, corresponding to the excess " AND ", then add the limit
            req = req.Remove(req.Length - 5) + " LIMIT 1";

            try
            {
                // Creates the command object
                using (MySqlCommand comm = new())
                {
                    // Opens the connection to the DB
                    Connection.Open();

                    // Setups the command
                    comm.CommandText = req;
                    comm.CommandType = CommandType.Text;
                    comm.Connection = Connection;

                    // Executes the command
                    comm.ExecuteNonQuery();

                    // Closes the connection
                    Connection.Close();
                }
            }
            catch (Exception e)
            {
                // Writes exception if caught
                Trace.WriteLine(e);
            }
        }

        /// <summary>
        /// Delete one or more rows, based on a = condition.
        /// </summary>
        /// <param name="columnToCheck"></param>
        /// <param name="valueToCheck"></param>
        public void DeleteRows(string columnToCheck, object valueToCheck)
        {
            // Builds the query
            string req = $"DELETE FROM {Name} WHERE {columnToCheck} = {valueToCheck}";

            try
            {
                // Creates the command object
                using (MySqlCommand comm = new())
                {
                    // Opens the connection to the DB
                    Connection.Open();

                    // Setups the command
                    comm.CommandText = req;
                    comm.CommandType = CommandType.Text;
                    comm.Connection = Connection;

                    // Executes the command
                    comm.ExecuteNonQuery();

                    // Closes the connection
                    Connection.Close();
                }
            }
            catch (Exception e)
            {
                // Writes exception if caught
                Trace.WriteLine(e);
            }
        }
        /// <summary>
        /// Delete one or more rows, based on multiple = conditions.
        /// </summary>
        /// <param name="columnToCheck"></param>
        /// <param name="valueToCheck"></param>
        public void DeleteRows(string[] columnsToCheck, object[] valuesToCheck)
        {
            // Builds the query
            string req = $"DELETE FROM {Name} WHERE ";

            // Iterates through the parameters to add their condition to the query
            for (int i = 0; i < columnsToCheck.Length; i++)
            {
                req += $"{columnsToCheck[i]} = {valuesToCheck[i]} AND ";
            }
            // Removes the last 5 chars, corresponding to the excess " AND "
            req = req.Remove(req.Length - 5);

            try
            {
                // Creates the command object
                using (MySqlCommand comm = new())
                {
                    // Opens the connection to the DB
                    Connection.Open();

                    // Setups the command
                    comm.CommandText = req;
                    comm.CommandType = CommandType.Text;
                    comm.Connection = Connection;

                    // Executes the command
                    comm.ExecuteNonQuery();

                    // Closes the connection
                    Connection.Close();
                }
            }
            catch (Exception e)
            {
                // Writes exception if caught
                Trace.WriteLine(e);
            }
        }


        // =====[ SELECT ]=====
        /// <summary>
        /// Selects a single row based on a = condition
        /// </summary>
        /// <param name="columnToCheck"></param>
        /// <param name="valueToCheck"></param>
        /// <returns>Returns a Dictionary(string, object) as ("column name", value) representing the returned row.</returns>
        public Dictionary<string, object> SelectRow(string columnToCheck, object valueToCheck)
        {
            // Builds the query
            string req = $"SELECT * FROM {Name} WHERE {columnToCheck} = @valueToCheck";

            

            // Gets list of columns to use later in filling the returned dictionary
            List<string> columns = GetColumnsNames();

            // Creates the returned dictionary
            Dictionary<string, object> returnDict = [];

            try
            {
                // Creates the command object
                using (MySqlCommand comm = new MySqlCommand())
                {
                    // Opens the connection to the DB
                    Connection.Open();

                    // Setups the command
                    comm.CommandText = req;
                    comm.CommandType = CommandType.Text;
                    comm.Connection = Connection;

                    comm.Parameters.Add(new MySqlParameter("@valueToCheck", valueToCheck));

                    // Creates the reader object that will read and store the data
                    MySqlDataReader reader = comm.ExecuteReader();

                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {   // Adds read values to the dict associated with their respective column name
                            returnDict.Add(columns[i], reader.GetValue(i));
                        }

                        // Breaks so it only selects one row
                        break;
                    }

                    // Closes the connection to the DB
                    Connection.Close();

                    // Returns the dict
                    return returnDict;
                }
            }
            catch (Exception e)
            {
                // Writes exception if caught
                Trace.WriteLine(e);
                Trace.WriteLine("Request was: " + req);
                Trace.WriteLine(" --> Returning empty Dictionary.");

                // Returns empty dict
                return new Dictionary<string, object>();
            }
        }

        /// <summary>
        /// Selects a single row based on multiples = conditions, takes two arrays representing the multiple columns and values to check.
        /// </summary>
        /// <param name="columnsToCheck"></param>
        /// <param name="valuesToCheck"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Returns a Dictionary(string, object) as ("column name", value) representing the returned row.</exception>
        public Dictionary<string, object> SelectRow(string[] columnsToCheck, object[] valuesToCheck)
        {
            // Verifies that there are the same amount of values and columns to check
            if(columnsToCheck.Length != valuesToCheck.Length)
            {
                throw new Exception("MySqlManager.Table Exception: Amount of columns to check does not match amount of values to match.");
            }

            // Builds the base query
            string req = $"SELECT * FROM {Name} WHERE ";

            // Iterates through the parameters to add their condition to the query
            for(int i = 0; i < columnsToCheck.Length; i++)
            {
                req += $"{columnsToCheck[i]} = {valuesToCheck[i]} AND ";
            }
            // Removes the last 5 chars, corresponding to the excess " AND "
            req = req.Remove(req.Length - 5);

            // Gets list of columns to use later in filling the returned dictionary
            List<string> columns = GetColumnsNames();

            // Creates the returned dictionary
            Dictionary<string, object> returnDict = [];

            try
            {
                // Creates the command object
                using (MySqlCommand comm = new MySqlCommand())
                {
                    // Opens the connection to the DB
                    Connection.Open();

                    // Setups the command
                    comm.CommandText = req;
                    comm.CommandType = CommandType.Text;
                    comm.Connection = Connection;

                    // Creates the reader object that will read and store the data
                    MySqlDataReader reader = comm.ExecuteReader();

                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {   // Adds read values to the dict associated with their respective column name
                            returnDict.Add(columns[i], reader.GetValue(i));
                        }

                        // Breaks so it only selects one row
                        break;
                    }

                    // Closes the connection to the DB
                    Connection.Close();

                    // Returns the dict
                    return returnDict;
                }
            }
            catch (Exception e)
            {
                // Writes exception if caught
                Trace.WriteLine(e);
                Trace.WriteLine(" Exception caught --> Returning empty Dictionary.");

                // Returns empty dict
                return new Dictionary<string, object>();
            }


        }
        
        /// <summary>
        /// Selects multiple rows based on a = conditions
        /// </summary>
        /// <param name="columnToCheck"></param>
        /// <param name="valueToCheck"></param>
        /// <returns>Returns a List of Dictionaries(string, object) with ("column name", value) representing the corresponding rows.</returns>
        public List<Dictionary<string, object>> SelectRows(string columnToCheck, object valueToCheck)
        {
            // Builds the query
            string req = $"SELECT * FROM {Name} WHERE {columnToCheck} = {valueToCheck}";

            // Gets list of columns to use later in filling the returned dictionary
            List<string> columns = GetColumnsNames();

            // Creates the returned list of dictionaries
            List<Dictionary<string, object>> returnList = [];

            try
            {
                // Creates the command object
                using (MySqlCommand comm = new MySqlCommand())
                {
                    // Opens the connection to the DB
                    Connection.Open();

                    // Setups the command
                    comm.CommandText = req;
                    comm.CommandType = CommandType.Text;
                    comm.Connection = Connection;

                    // Creates the reader object that will read and store the data
                    MySqlDataReader reader = comm.ExecuteReader();

                    while (reader.Read())
                    {
                        Dictionary<string, object> dict = [];
                        for (int i = 0; i < reader.FieldCount; i++)
                        {   // Adds read values to the dict associated with their respective column name
                            dict.Add(columns[i], reader.GetValue(i));
                        }

                        returnList.Add(dict);
                    }

                    // Closes the connection to the DB
                    Connection.Close();

                    // Returns the dict
                    return returnList;
                }
            }
            catch (Exception e)
            {
                // Writes exception if caught
                Trace.WriteLine(e);
                Trace.WriteLine(" --> Returning empty Dictionary.");

                // Returns empty List
                return new List<Dictionary<string, object>>();
            }
        }

        /// <summary>
        /// Selects multiple rows based on multiples = conditions, takes two arrays representing the multiple columns and values to check.
        /// </summary>
        /// <param name="columnsToCheck">A <see cref="string"/></param>
        /// <param name="valuesToCheck">Any <see cref="object"/></param>
        /// <returns> Returns a <see cref="List{T}"/> of <see cref="Dictionary{string, object}"/>, {string, object} with {"column name", value}, representing the corresponding rows.</returns>
        /// <exception cref="Exception">Thrown if the lengthes of columnsToCheck and valuesToCheck do not match.</exception>
        public List<Dictionary<string, object>> SelectRows(string[] columnsToCheck, object[] valuesToCheck)
        {
            // Verifies that there are the same amount of values and columns to check
            if (columnsToCheck.Length != valuesToCheck.Length)
            {
                throw new Exception("MySqlManager.Table Exception: Amount of columns to check does not match amount of values to match.");
            }

            // Gets list of columns to use later in filling the returned dictionary
            List<string> columns = GetColumnsNames();
            // Creates the returned list of dictionaries
            List<Dictionary<string, object>> returnList = [];

            // Creates the query
            string req = $"SELECT * FROM {Name} WHERE ";

            // Iterates through the parameters to add their conditions to the query
            for (int i = 0; i < columnsToCheck.Length; i++)
            {
                req += $"{columnsToCheck[i]} = {valuesToCheck[i]} AND ";
            }
            // Removes the last 5 chars, corresponding to the excess " AND "
            req = req.Remove(req.Length - 5);

            try
            {
                // Creates the command object
                using (MySqlCommand comm = new MySqlCommand())
                {
                    // Opens the connection to the DB
                    Connection.Open();

                    // Setups the command
                    comm.CommandText = req;
                    comm.CommandType = CommandType.Text;
                    comm.Connection = Connection;

                    // Creates the reader object that will read and store the data
                    MySqlDataReader reader = comm.ExecuteReader();

                    while (reader.Read())
                    {
                        Dictionary<string, object> dict = [];
                        for (int i = 0; i < reader.FieldCount; i++)
                        {   // Adds read values to the dict associated with their respective column name
                            dict.Add(columns[i], reader.GetValue(i));
                        }

                        returnList.Add(dict);
                    }

                    // Closes the connection to the DB
                    Connection.Close();

                    // Returns the dict
                    return returnList;
                }
            }
            catch (Exception e)
            {
                // Writes exception if caught
                Trace.WriteLine(e);
                Trace.WriteLine(" --> Returning empty Dictionary.");

                // Returns empty List
                return new List<Dictionary<string, object>>();
            }
        }

    }
}
