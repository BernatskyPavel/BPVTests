using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BPVTests
{
    class QuestionsData {
        public string Text { get; set; }
        public List<AnswersData> Answs { get; set; }
        public int ACount { get; set; }
        public int Type { get; set; }
        public int Difficult { get; set; }
        public string image { get; set; }
        public bool DefaultImage { get; set; }
        public byte[] images { get; set; }
        public bool Empty {get;set;}

        public QuestionsData() {
            DefaultImage = true;
            image = @"pack://application:,,,/Icon/add.png";
            Answs = new List<AnswersData>();
            Text = string.Empty;
            Type = -1;
            Difficult = -1;
            Empty = true;
            ACount = 0;
        }

    }

    class AnswersData {
        public string Text { get; set; }
        public string image { get; set; }
        public byte[] images {get;set;}
        public bool TorF { get; set; }
        public int Pos { get; set; }
        public bool DefaultImage { get; set; }
        public AnswersData()
        {
            DefaultImage = true;
            Text = string.Empty;
            image = @"pack://application:,,,/Icon/add.png";
            TorF = false;
            Pos = 0;
        }
    }
}
