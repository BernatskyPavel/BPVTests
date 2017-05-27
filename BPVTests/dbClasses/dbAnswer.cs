using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace BPVTests
{
    [Table(Name ="Answers")]
    class dbAnswer
    {
        [Column(Name ="AnswerID",IsPrimaryKey = true,IsDbGenerated =true)]
        public int AID { get; set; }
        [Column(Name ="QuestionID")]
        public int QID { get; set; }
        [Column(Name ="AnswerText")]
        public string Text { get; set; }
        [Column(Name ="TorF")]
        public bool Check { get; set; }
        [Column (Name ="Picture")]
        public byte[] Picture { get; set; }
        [Column(Name ="DnD")]
        public string DnDAnswer { get; set;}

        public override string ToString()
        {
            return "" + AID + "-" + QID+"-"+Text + "-" + Check + "-" + DnDAnswer;
        }

    }
}
