using System;
using System.Collections.Generic;
using System.Linq;
using Console_Time_Manager.Framework;
using Console_Time_Manager.Models;

namespace Console_Time_Manager
{
    public enum TimeManagerColors
    {
        Error = ConsoleColor.Red,
        Title = ConsoleColor.Cyan,
        Separator = ConsoleColor.Yellow,
        Succes = ConsoleColor.Green,
        FormField = ConsoleColor.DarkGreen,
        FormDescription = ConsoleColor.DarkGray,
        ListDots = ConsoleColor.DarkCyan
    }
    class TimeManager
    {
        private bool Terminar { get; set; } = false;
        private bool CerrarSesión { get; set; } = false;
        private string SelectedAction { get; set; }
        private User User { get; set; }
        private List<Proyect> Proyects { get; set; }
        private List<Task> Tasks { get; set; }
        private List<Record> ActiveRecords { get; set; } = new List<Record>();
        private Dictionary<Proyect, List<Record>> TasksByProyects
        {
            get
            {
                Dictionary<Proyect, List<Record>> ordered = new Dictionary<Proyect, List<Record>>();

                foreach (Proyect proyect in Proyects)
                {
                    List<Record> collection = ActiveRecords.FindAll((Record at) => at.IdProyect == proyect.IdProyect);
                    if (collection.Count != 0)
                    {
                        ordered.Add(proyect, collection);
                    }
                }

                return ordered;
            }
        }
        public void Execute()
        {
            BANNER();
            while (!Terminar)
            {
                AccionesBase();
                switch (SelectedAction)
                {
                    case "4": Terminar = true; continue;
                    case "3": Console.Clear(); BANNER(); break;
                    case "2": RegisterRoutine(); break;
                    case "1": LogInRoutine(); break;
                }
                if (User != null)
                {
                    while (!CerrarSesión)
                    {
                        ActionsLogIn();
                    }
                    CerrarSesión = false;
                }
            }
        }
        private void StartRecord(long idProyect, long idTask = 0)
        {
            Record newRecord = new Record().Populate(User.IdUser, idProyect, idTask, null);
            newRecord.StartRecording();
            ActiveRecords.Add(newRecord);
        }
        private void StopRecord(int index)
        {
            Record record = ActiveRecords[index];

            Console.WriteLine($"Write a description for the record of {Tasks.Find(t => t.IdTask == ActiveRecords[index]?.IdTask)?.TaskTitle ?? Proyects.Find(p => p.IdProyect == ActiveRecords[index]?.IdProyect).ProyectName}.");
            ConsoleExtender.Description("Press enter if you don't want to add one.");
            string description = Console.ReadLine();
            record.RecordDescription= description;

            string sql =(record.IdTask > 0)? "INSERT INTO Record (IdUser, IdProyect, IdTask, RecordStart, RecordEnd, RecordDescription) VALUES (@IdUser, @IdProyect, @IdTask, @RecordStart, @RecordEnd, @RecordDescription)": "INSERT INTO Record (IdUser, IdProyect, RecordStart, RecordEnd, RecordDescription) VALUES (@IdUser, @IdProyect, @RecordStart, @RecordEnd, @RecordDescription)";
            try
            {
                ActiveRecords[index].StopRecording();
                SQLiteAccesLayer.Query(sql, record);
                ActiveRecords.RemoveAt(index);
            }
            catch (Exception e)
            {
                ConsoleExtender.LogError(e);
                return;
            }
            ConsoleExtender.Succes($"Stopped recording!");
        }

        private void AddProyect()
        {
            FormField[] formFields =
            {
                new FormField(
                    name: "Name",
                    type: FormFieldType.String,
                    required: true,
                    isInLogIn: false,
                    minimunLength: 1,
                    maximunLength: 255
                ),
                new FormField(
                    name: "Description",
                    type: FormFieldType.String,
                    required: false,
                    minimunLength: 0,
                    maximunLength: 255
                ),
                new FormField(
                    name: "Color",
                    type: FormFieldType.Radio,
                    required: true,
                    minimunLength: 0,
                    maximunLength: 1,
                    posibleAnswers: Enumerable.ToList(Enum.GetNames(typeof(ConsoleColor)))
                )
            };

            ConsoleExtender.CreateTitle("New Proyect");
            Dictionary<string, string> answers = ConsoleExtender.Form(new Form(formFields));
            
            if (!answers.TryGetValue("Name", out string title))
            {
                ConsoleExtender.Error("FORM TERMINATED"); return;
            }

            Proyect proyect = new Proyect();
            proyect.Populate(User.IdUser, title, (ConsoleColor)Enum.Parse(typeof(ConsoleColor), answers["Color"]), answers["Description"]);

            try
            {
                string sql = "INSERT INTO Proyect(IdUser, ProyectName, ProyectDescription, ProyectColor) VALUES (@IdUser, @ProyectName, @ProyectDescription, @ProyectColor)";
                (int _, long insertedId) = SQLiteAccesLayer.Query(sql, proyect);
                proyect.SetId(insertedId);
                Proyects.Add(proyect);
            }
            catch (Exception e)
            {
                ConsoleExtender.LogError(e);
                return;
            }
            ConsoleExtender.Succes("Proyect added succesfully!");
            ConsoleExtender.CreateSeparator();
        }
        private void UpdateProyect(Proyect oldProyect)
        {
            int index = Proyects.FindIndex(p => p.IdProyect == oldProyect.IdProyect);

            FormField[] fields =
            {
                new FormField(
                    name: "Name",
                    type: FormFieldType.String,
                    required: false,
                    minimunLength: 0,
                    maximunLength: 255,
                    defaultAnswer: oldProyect.ProyectName
                ),
                new FormField(
                    name: "Description",
                    type: FormFieldType.String,
                    required: false,
                    minimunLength: 0,
                    maximunLength: 255,
                    defaultAnswer: oldProyect.ProyectDescription ?? ""
                ),
                new FormField(
                    name: "Color",
                    type: FormFieldType.Radio,
                    required: false,
                    minimunLength: 0,
                    maximunLength: 255,
                    defaultAnswer: $"{(int)oldProyect.ProyectColor+1}",
                    posibleAnswers: Enumerable.ToList(Enum.GetNames(typeof(ConsoleColor)))
                )
            };
            ConsoleExtender.CreateLightSeparator();
            ConsoleExtender.CreateSubTitle($"Edit {oldProyect.ProyectName}");
            ConsoleExtender.Description("Press enter if you don't want to modify the value of that field.");
            Dictionary<string, string> answers = ConsoleExtender.Form(new Form(fields));

            if (!answers.TryGetValue("Name", out string name))
            {
                ConsoleExtender.Error("FORM TERMINATED");
                return;
            }

            Proyect newProyect = new Proyect();
            newProyect.Populate(oldProyect.IdUser, name, (ConsoleColor)Enum.Parse(typeof(ConsoleColor), answers["Color"]), answers["Description"]);
            newProyect.SetId(oldProyect.IdProyect);

            string sql = "UPDATE Proyect SET IdUser=@IdUser, ProyectName=@ProyectName, ProyectDescription=@ProyectDescription, ProyectColor=@ProyectColor WHERE IdProyect=@IdProyect";
            try
            {
                SQLiteAccesLayer.Query(sql, newProyect);
                Proyects[index] = newProyect;
                ConsoleExtender.Succes("Proyect Modified Succesfully!");
            }
            catch (Exception e)
            {
                ConsoleExtender.Error("Error modifiying proyect");
                ConsoleExtender.LogError(e);
                return;
            }

        }
        private void DeleteProyect(Proyect proyect)
        {
            while (true)
            {
                ConsoleExtender.Error("Are you sure that you want to delete this proyect? (y/n)");
                ConsoleExtender.Description("You can't undo this action.");
                string answer = Console.ReadLine();
                if (answer == "y")
                {
                    try
                    {
                        SQLiteAccesLayer.Query("DELETE FROM Record WHERE IdProyect=@IdProyect", proyect);
                        SQLiteAccesLayer.Query("DELETE FROM Task WHERE IdProyect=@IdProyect", proyect);
                        SQLiteAccesLayer.Query("DELETE FROM Proyect WHERE IdProyect=@IdProyect", proyect);
                        ActiveRecords.RemoveAll(ar => ar.IdProyect == proyect.IdProyect);
                        Tasks.RemoveAll(t => t.IdProyect == proyect.IdProyect);
                        Proyects.Remove(proyect);

                        ConsoleExtender.Succes($"Proyect: {proyect.ProyectName}, deleted succesfully!");
                        return;
                    }
                    catch (Exception e)
                    {
                        ConsoleExtender.LogError(e);
                        return;
                    }
                }
                if (answer == "n")
                {
                    return;
                }
            }
        }
        private void AddTask(long idProyect)
        {
            FormField[] formFields =
            {
                new FormField(
                    name: "Title",
                    type: FormFieldType.String,
                    required: true,
                    minimunLength: 1,
                    maximunLength: 255
                ),
                new FormField(
                    name: "Description",
                    type: FormFieldType.String
                )
            };
            ConsoleExtender.CreateSubTitle("Add a task");
            
            Dictionary<string, string> answers = ConsoleExtender.Form(new Form(formFields));
            if (!answers.TryGetValue("Title", out string title))
            {
                ConsoleExtender.Error("FORM TERMINATED"); return;
            }

            Task task = new Task();
            task.Populate(idProyect, title, answers["Description"] ?? "");
            
            try
            {
                string sql = "INSERT INTO Task(IdProyect, TaskTitle, TaskDescription) VALUES (@IdProyect, @TaskTitle, @TaskDescription)";
                (int _, long insertedId) = SQLiteAccesLayer.Query(sql, task);
                task.SetId(insertedId);
                Tasks.Add(task);
            }
            catch (Exception e)
            {
                ConsoleExtender.LogError(e);
                return;
            }

            ConsoleExtender.Succes($"Task added to {Proyects.Find(p=>p.IdProyect == idProyect).ProyectName} succesfully!");
            ConsoleExtender.CreateSeparator();
        }

        private void ManageProyects()
        {
            ConsoleExtender.CreateTitle("PROYECTS");
            try
            {
                List<Proyect> proyects = SQLiteAccesLayer.Get<Proyect>($"SELECT * FROM Proyect WHERE IdUser={User.IdUser}");
                if (proyects.Count == 0)
                {
                    Console.WriteLine("THERE ARE NO PROYECTS.");
                    Console.WriteLine("Please create one, go back to main menú");
                    return;
                }

                List<FormFieldAction> actions = new List<FormFieldAction>
                {
                    new FormFieldAction("1", "Go Back")
                };
                for (int i = 2; i < proyects.Count+2; i++)
                {
                    actions.Add(new FormFieldAction($"{i}", proyects[i-2].ProyectName, proyects[i-2].ProyectDescription, proyects[i-2].IdProyect));
                }

                string answer = ConsoleExtender.Actions(actions.ToArray());
                if (answer == "1") { return; }
                Proyect Proyect = proyects.Find(p => p.IdProyect == actions.Find(a => a.Index == answer).Id);
                ManageProyect(Proyect);
            }
            catch (Exception e)
            {
                ConsoleExtender.LogError(e);
                return;
            }
            ConsoleExtender.CreateSeparator();
            //Console.WriteLine("Pending to implement.");
        }
        private void ManageProyect(Proyect proyect)
        {
            ConsoleExtender.CreateLightSeparator();
            ConsoleExtender.CreateSubTitle(proyect.ProyectName);
            List<Task> tasks = SQLiteAccesLayer.Get<Task>($"SELECT * FROM Task WHERE IdProyect = {proyect.IdProyect}");
            
            List<FormFieldAction> tasksByIndex = new List<FormFieldAction>();
            if (tasks.Count==0)
            {
                Console.WriteLine("NO TASKS");
                Console.WriteLine("This proyect doens't has any taks, press 3 to add one!");
            } else
            {
                ConsoleExtender.ShowTask_Records(tasks, proyect.ProyectColor, out tasksByIndex);
            }

            List<FormFieldAction> actions = new List<FormFieldAction>
            {
                new FormFieldAction("1", "Go Back"),
                new FormFieldAction("2", "Start Recording Proyect", "Starts the recording of this proyect."),
                new FormFieldAction("3", "Add Task", "Add a task to the proyect."),
                new FormFieldAction("4", "Edit Proyect", "Let's you edit the Title, description and color."),
                new FormFieldAction("5", "Delete Proyect", "Permanently delete de proyect.")
            };

            string answer = ConsoleExtender.Actions(actions.ToArray());
            if (!int.TryParse(answer, out int number))
            {
                long? selectedId = tasksByIndex.Find(ta => ta.Index == answer)?.Id;
                if (selectedId != null)
                {
                    Record r = new Record().Populate(User.IdUser, proyect.IdProyect, (long)selectedId, null);
                    r.StartRecording();
                    ActiveRecords.Add(r);
                }
            }
            switch (number)
            {
                case 1:
                    return;
                case 2:
                    if (ActiveRecords.FindIndex(ar => ar.IdProyect == proyect.IdProyect) < 0)
                    {
                        StartRecord(proyect.IdProyect);
                    }
                    break;
                case 3:
                    AddTask(proyect.IdProyect);
                    break;
                case 4:
                    UpdateProyect(proyect);
                    break;
                case 5:
                    DeleteProyect(proyect);
                    break;
            }

        }
        private void Report()
        {
            ConsoleExtender.CreateTitle("Report");
            FormFieldAction[] actions =
            {
                new FormFieldAction("1", "Go Back"),
                new FormFieldAction("2", "Today", "Gives you a report on how you inverted your time today."),
                new FormFieldAction("3", "Week", "Report that includes records 7 days before today."),
                new FormFieldAction("4", "Monthly", "Report of the month"),
                new FormFieldAction("5", "Year", "Report of the year"),
                new FormFieldAction("6", "Lifetime", "Includes all existing records.")
            };

            List<FormFieldAction> subActions = new List<FormFieldAction>
            {
                new FormFieldAction("1", "Go Back"),
                new FormFieldAction("2", "ALL")
            };
            for (int i = 3; i < Proyects.Count+3; i++)
            {
                subActions.Add(new FormFieldAction($"{i}", Proyects[i-3].ProyectName, Proyects[i-3].ProyectDescription, Proyects[i-3].IdProyect));
            }

            DateTime startDate = new DateTime();
            while (true)
            {
                string answer = ConsoleExtender.Actions(actions);
                if (int.TryParse(answer, out int numberAnswer))
                {
                    switch (numberAnswer)
                    {
                        case 1:
                            return;
                        case 2:
                            startDate = DateTime.Today;
                            break;
                        case 3:
                            startDate = DateTime.Today.Subtract(TimeSpan.FromDays(7));
                            break;
                        case 4:
                            startDate = DateTime.Today.Subtract(TimeSpan.FromDays(30));
                            break;
                        case 5:
                            startDate = DateTime.Today.Subtract(TimeSpan.FromDays(365));
                            break;
                    }
                    while (true)
                    {
                        answer = ConsoleExtender.Actions(subActions.ToArray());
                        long? selectedId = Proyects.Find(p => p.IdProyect == subActions.Find(sa => sa.Index == answer)?.Id)?.IdProyect;
                        List<Proyect> proyects = new List<Proyect>();
                        Proyect proyect = null;
                        List<Task> tasks = new List<Task>();
                        List<ReportTableRow> table = new List<ReportTableRow>();
                        string title = "";
                        if (int.TryParse(answer, out numberAnswer))
                        {
                            switch (numberAnswer)
                            {
                                case 1:
                                    return;
                                case 2:
                                    proyects = Proyects;
                                    title = "General";
                                    break;
                                default:
                                    tasks = Tasks.FindAll(p => p.IdProyect == selectedId);
                                    proyect = Proyects.Find(p => p.IdProyect == selectedId);
                                    title = proyect.ProyectName;
                                    break;
                            }
                            if (proyects.Count != 0)
                            {
                                table = MakeReport(proyects, startDate);
                            }
                            else if (proyect != null)
                            {
                                table = MakeReport(proyect.IdProyect, tasks, startDate, proyect.ProyectColor);
                            }
                            ConsoleExtender.ShowReport(table, title);
                            return;
                        }
                    }
                }
            }
        }
        private List<ReportTableRow> MakeReport(List<Proyect> proyects, DateTime startDate)
        {
            List<ReportTableRow> table = new List<ReportTableRow>();
            try
            {
                foreach (Proyect proyect in proyects)
                {
                    List<Record> records = SQLiteAccesLayer.Get<Record>($"SELECT * FROM Record WHERE IdUser={User.IdUser} AND IdProyect={proyect.IdProyect} AND RecordStart>=date('{startDate.ToString("yyyy-MM-dd HH:mm:ss.sss")}')");
                    TimeSpan timeRecorded = TimeSpan.FromMilliseconds(records.Select(r => (r.RecordEnd - r.RecordStart).TotalMilliseconds).Sum());
                    var row = new ReportTableRow(
                        title: proyect.ProyectName,
                        description: proyect.ProyectDescription,
                        recordedTime: timeRecorded.ToString("g"),
                        color: proyect.ProyectColor
                    );
                    table.Add(row);
                }
            }
            catch (Exception e)
            {
                ConsoleExtender.LogError(e);
                throw e;
            }
            return table;
        }
        private List<ReportTableRow> MakeReport(long idProyect, List<Task> tasks, DateTime startDate, ConsoleColor color)
        {
            List<ReportTableRow> table = new List<ReportTableRow>();
            try
            {
                List<Record> proyectRecords = SQLiteAccesLayer.Get<Record>($"SELECT * FROM Record WHERE IdUser={User.IdUser} AND IdProyect={idProyect} AND RecordStart>=date('{startDate.ToString("yyyy-MM-dd HH:mm:ss.sss")}')");
                table.Add(new ReportTableRow(
                    title: "-- No Task --",
                    description: "Time recorded with no task asigned",
                    recordedTime: TimeSpan.FromMilliseconds(proyectRecords.Select(r => (r.RecordEnd - r.RecordStart).TotalMilliseconds).Sum()).ToString("g"),
                    color: color
                ));
                foreach (Task task in tasks)
                {
                    List<Record> records = SQLiteAccesLayer.Get<Record>($"SELECT * FROM Record WHERE IdUser={User.IdUser} AND IdProyect={task.IdProyect} AND IdTask={task.IdTask} AND RecordStart>=date('{startDate.ToString("yyyy-MM-dd HH:mm:ss.sss")}')");
                    TimeSpan timeRecorded = TimeSpan.FromMilliseconds(records.Select(r => (r.RecordEnd - r.RecordStart).TotalMilliseconds).Sum());
                    var row = new ReportTableRow(
                        title: task.TaskTitle,
                        description: task.TaskDescription,
                        recordedTime: timeRecorded.ToString("g"),
                        color: color
                    );
                    table.Add(row);
                }
            }
            catch (Exception e)
            {
                ConsoleExtender.LogError(e);
                throw e;
            }
            return table;
        }
        private void LogOut()
        {
            CerrarSesión = true;            
            ConsoleExtender.CreateTitle("LOG OUT");
            ConsoleExtender.CreateLightSeparator();
            ConsoleExtender.CreateSubTitle("Stopping Recordings...");
            for (int i = 0; i < ActiveRecords.Count; i++)
            {
                StopRecord(i);
            }
            ConsoleExtender.Succes($"Succesfully Logged Out, Good Bye {User.UserName}");
            Tasks = new List<Task>();
            Proyects = new List<Proyect>();
            User = null;
            ConsoleExtender.CreateLightSeparator();
        }

        private void BANNER()
        {
            ConsoleExtender.CreateSeparator();
            ConsoleExtender.BANNER(ConsoleExtender.WelcomeBanner);
            ConsoleExtender.CreateSeparator();
            Console.WriteLine("Welcome to the Console Time Manager!");
        }
        private void AccionesBase()
        {
            FormFieldAction[] actions =
            {
                new FormFieldAction("1","LogIn"),
                new FormFieldAction("2", "Register"),
                new FormFieldAction("3", "Clear Console"),
                new FormFieldAction("4", "Exit"),
            };
            Console.WriteLine("What do you want to do?");
            SelectedAction = ConsoleExtender.Actions(actions);
        }
        private void ActionsLogIn()
        {
            FormFieldAction[] actions =
            {
                new FormFieldAction("1", "Refresh", "Refreshes the time showed on the proyects."),
                new FormFieldAction("2", "Add Proyect", "Create a new proyect."),
                new FormFieldAction("3", "Manage Proyects", "Let's you manage your existing proyects."),
                new FormFieldAction("4", "Report", "Gives statistics on how you inverted your time."),
                new FormFieldAction("5", "Clear Console", "Clears the console."),
                new FormFieldAction("6", "LogOut", "Stops all recordings and LogOuts."),
            };
            ConsoleExtender.CreateTitle(User.UserName);
            ConsoleExtender.CreateSubTitle("Proyects Recording...");
            ConsoleExtender.Description("Press the index to stop recording.");
            ConsoleExtender.ShowRecordingTasks(TasksByProyects, Tasks, out List<FormFieldAction> tasksWithIndex);
            
            ConsoleExtender.CreateLightSeparator();
            
            SelectedAction = ConsoleExtender.Actions(actions);
            if (!int.TryParse(SelectedAction, out int selectedNumberAction))
            {
                long? selectedId = tasksWithIndex.Find(ta => ta.Index == SelectedAction)?.Id;
                if (SelectedAction.Length == 2)
                {
                    int index = ActiveRecords.FindIndex(r => r.IdTask == selectedId);
                    StopRecord(index);
                } else
                {
                    List<Record> proyectActiveRecords = ActiveRecords.FindAll(ar => ar.IdProyect == selectedId && ar.Active == true);
                    foreach (Record record in proyectActiveRecords)
                    {
                        int index = ActiveRecords.FindIndex(ar => ar.Equals(record));
                        StopRecord(index);
                    }
                }
            }
            switch (selectedNumberAction)
            {
                case 6:
                    LogOut();
                    break;
                case 5:
                case 1:
                    Console.Clear();
                    break;
                case 4:
                    Report();
                    break;
                case 3:
                    ManageProyects();
                    break;
                case 2:
                    AddProyect();
                    break;
            }
        }
        private void RegisterRoutine()
        {
            FormField[] formFields =
            {
                new FormField(
                    name: "Email",
                    type: FormFieldType.Email,
                    required: true
                ),
                new FormField(
                    name: "Username",
                    type: FormFieldType.String,
                    required: true,
                    minimunLength: 0,
                    maximunLength: 64
                ),
                new FormField(
                    name: "Password",
                    type: FormFieldType.Password,
                    required: true,
                    isInLogIn: true,
                    minimunLength: 8,
                    maximunLength: 16
                )
            };
            Form form = new Form(formFields);
            
            ConsoleExtender.CreateTitle("REGISTER");
            Dictionary<string, string> answers =  ConsoleExtender.Form(form);
            
            if (!answers.TryGetValue("Email", out string _))
            {
                ConsoleExtender.Error("FORM TERMINATED");
                return;
            }
            User newUser = new User();
            newUser.Populate(answers["Username"], answers["Email"], answers["Password"]);
            try
            {
                (int _, long insertedId) = SQLiteAccesLayer.Query("INSERT INTO User(UserName, Email, Password) VALUES (@UserName, @Email, @Password)", newUser);
                newUser.SetId(insertedId);
                ConsoleExtender.Succes("Congratulations! You registered succesfully!\nNow please LogIn to your account.");
            }
            catch (Exception e)
            {
                ConsoleExtender.LogError(e);
                return;
            }
            ConsoleExtender.CreateSeparator();
        }
        private void LogInRoutine()
        {
            FormField[] formFields =
            {
                new FormField(
                    name: "Email",
                    type: FormFieldType.Email,
                    required: true,
                    isInLogIn: true,
                    minimunLength: 0,
                    maximunLength: 64
                ),
                new FormField(
                    name: "Password",
                    type: FormFieldType.Password,
                    required: true,
                    minimunLength: 8,
                    maximunLength: 16
                )
            };
            Form form = new Form(formFields);

            ConsoleExtender.CreateTitle("LOGIN");
            Dictionary<string, string> answers = ConsoleExtender.Form(form);

            if (!answers.TryGetValue("Email", out string email))
            {
                ConsoleExtender.Error("FORM TERMINATED"); return;
            }

            List<User> user;
            try
            {
                user = SQLiteAccesLayer.Get<User>($"SELECT * FROM User WHERE Email='{email}' LIMIT 1");
            }
            catch (Exception e)
            {
                ConsoleExtender.LogError(e);
                return;
            }
            if (user.Count() == 0)
            {
                ConsoleExtender.Error("The acount you tried to LogIn doesn't exists!"); return;
            }
            string encriptedInputPwd = answers["Password"];
            if (user[0].Password != encriptedInputPwd)
            {
                ConsoleExtender.Error("Incorrect Password!"); return;
            }

            Tasks = SQLiteAccesLayer.Get<Task>($"SELECT * FROM Task AS T INNER JOIN Proyect AS P ON T.IdProyect=P.IdProyect WHERE IdUser={user[0].IdUser}");
            Proyects = SQLiteAccesLayer.Get<Proyect>($"SELECT * FROM Proyect WHERE IdUser={user[0].IdUser}");

            User = user[0];
            ConsoleExtender.Succes($"You are now logged in as {User.UserName}!");
        }
    }
}
