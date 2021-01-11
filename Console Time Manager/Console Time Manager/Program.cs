using System;
using System.Collections.Generic;
using System.Text;

namespace Console_Time_Manager
{
    class Program
    {
        public static TimeManager TM { get; set; } = new TimeManager();
        public static void Main(string[] args)
        {
            TM.Execute();
        }
    }
}
