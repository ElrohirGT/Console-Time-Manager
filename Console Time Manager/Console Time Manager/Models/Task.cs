using System;
using System.Collections.Generic;
using System.Text;

namespace Console_Time_Manager.Models
{
    class Task
    {
        public long IdProyect { get; private set; }
        public long IdTask { get; private set; }
        public string TaskTitle { get; private set; }
        public string TaskDescription { get; private set; }
        public void Populate(long idProyect, string title, string description)
        {
            IdProyect = idProyect;
            TaskTitle = title;
            TaskDescription = description;
        }
        public void SetId(long id)
        {
            IdTask = id;
        }
    }
}
