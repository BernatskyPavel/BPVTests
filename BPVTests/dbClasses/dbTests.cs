using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace BPVTests
{
    [Table(Name ="Tests")]
    class dbTests
    {
        [Column(Name ="TestID")]
        public int TestID { get;  set; }
        [Column(Name ="QuestionID")]
        public int QuestionID { get; set; }
    }
}
