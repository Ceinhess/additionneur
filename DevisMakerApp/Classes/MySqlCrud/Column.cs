using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevisMakerApp.Classes.MySqlCrud
{
    public class Column
    {
        public string Name { get; }

        public string Type { get; }

        public Table Parent { get; }

        MySqlConnection Connection = null;

        public Column(string connectionString, string _name, string _type, Table _parent)
        {
            Name = _name;
            Type = _type;
            Parent = _parent;

            //Connection = new MySqlConnection(connectionString);
        }
    }
}
