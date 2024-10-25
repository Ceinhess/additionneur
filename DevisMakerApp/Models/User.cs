using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Additionneur.Models
{
    internal class User
    {
        public int Id { get; set; }
        
        public string Username { get; set; }
        public string Email { get; set; }

        public User(int id, string username, string email) 
        {
            Id = id;
            Username = username;
            Email = email;
        }

    }
}
