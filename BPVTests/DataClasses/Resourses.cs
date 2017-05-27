using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BPVTests
{
    static class Resourses
    {
        public static string connectionS = @"Integrated Security=SSPI;Initial Catalog=BPVTests_2017";
        public static int CurrentTestID = -1;
        public static int CurrentQuestionID = -1;
        public static int CurrentQuestionNum = -1;
        public static int ChangeBox = -1;

        public static bool FisherQ = false;
        public static bool FisherA = false;
        public static bool StrongQ = false;
        public static bool TimeQ = false;
        public static bool DifficultQ = false;
        public static TimeSpan TimeP = new TimeSpan(0,0,0);
        public static int FormatID = -1;

        public static List<dbQuestion> CurrentTestUser = new List<dbQuestion>();
        public static List<dbAnswer> CurrentAnswerstUser = new List<dbAnswer>();
        public static int Min = 0;

    }
}
