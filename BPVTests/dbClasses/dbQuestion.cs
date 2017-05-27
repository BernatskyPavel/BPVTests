using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace BPVTests
{
    [Table(Name ="Questions")]
    class dbQuestion
    {
        [Column(Name ="QuestionID",IsPrimaryKey =true,IsDbGenerated =true)]
        public int QuestionID { get; set; }
        [Column(Name ="QuestionText")]
        public string Text { get; set; }
        [Column(Name ="QuestionDifficult")]
        public int Difficult { get; set; }
        [Column(Name = "QuestionType")]
        public int Type { get; set; }
        [Column(Name ="AnswersCount")]
        public int AnswersCount { get; set; }
        [Column(Name ="Picture")]
        public byte[] Picture { get; set; }

        public override string ToString()
        {
            return "" + QuestionID + "-" + Text + "-" + Difficult + "-" + Type+"-"+AnswersCount;
        }

    }
}
