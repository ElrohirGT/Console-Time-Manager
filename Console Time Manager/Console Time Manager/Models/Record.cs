using System;
using System.Collections.Generic;
using System.Text;

namespace Console_Time_Manager.Models
{
    class Record
    {
        public DateTime RecordStart { get; private set; }
        public DateTime RecordEnd { get; private set; }
        public bool Active { get; private set; }
        public long IdUser { get; private set; }
        public long IdProyect { get; private set; }
        public long IdTask { get; private set; }
        public string RecordDescription { get; set; }
        public Record Populate(long idUser, long idProyect, long idTask = 0, string description = "")
        {
            IdUser = idUser;
            IdProyect = idProyect;
            IdTask = idTask;
            RecordDescription = description;
            StartRecording();
            return this;
        }
        public bool StartRecording()
        {
            if (RecordStart.Equals(new DateTime()))
            {
                RecordStart = DateTime.Now;
                Active = true;
                return true;
            }
            return false;
        }
        public bool StopRecording()
        {
            if (RecordStart < DateTime.Now)
            {
                RecordEnd = DateTime.Now;
                Active = false;
                return true;
            }
            return false;
        }
    }
}
