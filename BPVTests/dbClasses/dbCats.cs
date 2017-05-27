using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace BPVTests
{
    [Table(Name ="Categories")]
    class dbCats
    {
        [Column(Name = "CategoryName", IsPrimaryKey = true)]
        public string CatName { get; set; }
    }
}
