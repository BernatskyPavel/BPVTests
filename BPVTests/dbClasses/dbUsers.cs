using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Linq.Mapping;

namespace BPVTests
{
    [Table(Name="Users")]
    class dbUsers
    {
        [Column(IsPrimaryKey = true,Name ="Username")]
        public string Username { get; set; }
        [Column(Name = "Password")]
        public string Password { get; set; }
        [Column(Name = "UserType")]
        public bool AccType { get; set; }
    }
}
