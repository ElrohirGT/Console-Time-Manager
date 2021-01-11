using System;
using System.Collections.Generic;
using System.Text;

namespace Console_Time_Manager.Models
{
    public class Proyect
    {
        public long IdUser { get; private set; }
        public long IdProyect { get; private set; }
        public string ProyectName { get; private set; }
        public string ProyectDescription { get; private set; }
        public ConsoleColor ProyectColor { get; private set; }
        public DateTime StartDate { get; set; }
        public void Populate(long idUser, string title, ConsoleColor color, string description = "")
        {
            IdUser = idUser;
            ProyectName = title;
            ProyectDescription = description;
            ProyectColor = color;
        }
        public void SetId(long id)
        {
            IdProyect = id;
        }
    }
}
