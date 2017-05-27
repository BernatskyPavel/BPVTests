using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace BPVTests
{
    [Table(Name ="Test")]
    class dbTest
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, Name = "TestID")]
        public int ID { get; set; }
        [Column(Name = "TestName")]
        public string TestName { get; set; }
        [Column(Name = "QuestionCount")]
        public int QuestionCount { get; set; }
        [Column(Name = "Category")]
        public string Category { get; set; }
        [Column(Name ="MinSuccess")]
        public int MinSuccess { get; set; }
        public override string ToString()
        {
            return ""+ID+"-"+TestName+"-"+QuestionCount+"-"+Category;
        }
    }
}
