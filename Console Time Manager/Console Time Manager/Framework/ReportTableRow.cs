using System;
using System.Collections.Generic;
using System.Text;

namespace Console_Time_Manager.Framework
{
    class ReportTableRow
    {
        public ConsoleColor ProyectColor { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string RecordedTime { get; private set; }
        public ReportTableRow(string title, string description, string recordedTime, ConsoleColor color)
        {
            Title = title;
            Description = description;
            RecordedTime = recordedTime;
            ProyectColor = color;
        }
    }
}
