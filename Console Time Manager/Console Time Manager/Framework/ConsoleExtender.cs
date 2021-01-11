using System;
using System.Collections.Generic;
using BetterConsoleTables;
using Console_Time_Manager.Models;

namespace Console_Time_Manager.Framework
{
    static class ConsoleExtender
    {
        static readonly string Separator = "=";
        static readonly string LightSeparator = "-";
        static readonly string RequiredSymbol = "*";
        public static string[] WelcomeBanner =
        {
            "████████╗██╗███╗   ███╗███████╗    ███╗   ███╗ █████╗ ███╗   ██╗ █████╗  ██████╗ ███████╗██████╗ ",
            "╚══██╔══╝██║████╗ ████║██╔════╝    ████╗ ████║██╔══██╗████╗  ██║██╔══██╗██╔════╝ ██╔════╝██╔══██╗",
            "   ██║   ██║██╔████╔██║█████╗      ██╔████╔██║███████║██╔██╗ ██║███████║██║  ███╗█████╗  ██████╔╝",
            "   ██║   ██║██║╚██╔╝██║██╔══╝      ██║╚██╔╝██║██╔══██║██║╚██╗██║██╔══██║██║   ██║██╔══╝  ██╔══██╗",
            "   ██║   ██║██║ ╚═╝ ██║███████╗    ██║ ╚═╝ ██║██║  ██║██║ ╚████║██║  ██║╚██████╔╝███████╗██║  ██║",
            "   ╚═╝   ╚═╝╚═╝     ╚═╝╚══════╝    ╚═╝     ╚═╝╚═╝  ╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝ ╚═════╝ ╚══════╝╚═╝  ╚═╝"
        };
        static readonly string[] LowerLetters = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "ñ", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        static readonly string[] UpperLetters = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "Ñ", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        static int LineLength
        {
            get
            {
                return WelcomeBanner[0].Length;
            }
        }
        static int LineMiddle
        {
            get
            {
                return (int)Math.Ceiling((double)LineLength / 2);
            }
        }
        public static void CreateSeparator()
        {
            Console.ForegroundColor = (ConsoleColor)TimeManagerColors.Separator;
            string line = "";
            for (int j = 0; j < LineLength; j++)
            {
                line += Separator;
            }
            Console.WriteLine(line);
            Console.ResetColor();
        }
        public static void CreateLightSeparator()
        {
            Console.ForegroundColor = (ConsoleColor)TimeManagerColors.Separator;
            string line = "";
            for (int j = 0; j < LineLength; j++)
            {
                line += LightSeparator;
            }
            Console.WriteLine(line);
            Console.ResetColor();
        }
        public static void Error(string txt = "An Error Ocurred!")
        {
            Console.ForegroundColor = (ConsoleColor)TimeManagerColors.Error;
            Console.WriteLine(txt);
            Console.ResetColor();
        }
        public static void LogError(Exception e)
        {
            Error($"MESSAGE:\n{e.Message}");
            Error($"SOURCE:\n{e.Source}");
            Error($"ERROR:\n{e.ToString()}");
        }
        public static void Succes(string txt)
        {
            Console.ForegroundColor = (ConsoleColor)TimeManagerColors.Succes;
            Console.WriteLine(txt);
            Console.ResetColor();
        }
        public static void Description(string txt)
        {
            Console.ForegroundColor = (ConsoleColor)TimeManagerColors.FormDescription;
            Console.WriteLine(txt);
            Console.ResetColor();
        }
        public static void BANNER(string[] banner)
        {
            Console.ForegroundColor = (ConsoleColor)TimeManagerColors.Title;
            foreach (string line in banner)
            {
                Console.WriteLine(line);
            }
            Console.ResetColor();
        }
        public static void CreateTitle(string title)
        {
            int titleLeft_Right = (int)Math.Floor(((double)title.Length+2) / 2);
            int titleMiddleChar = (title.Length % 2 != 0) ? 1 : 0;

            int subTitleHalf = LineMiddle - titleLeft_Right - titleMiddleChar;
            
            string separators = "";
            
            for (int i = 0; i < subTitleHalf; i++)
            {
                separators += Separator;
            }

            Console.ForegroundColor = (ConsoleColor)TimeManagerColors.Separator;
            Console.Write(separators);
            
            Console.ForegroundColor = (ConsoleColor)TimeManagerColors.Title;
            Console.Write($" {title.ToUpper()} ");

            Console.ForegroundColor = (ConsoleColor)TimeManagerColors.Separator;
            Console.WriteLine(separators);

            Console.ResetColor();
        }
        public static void CreateSubTitle(string title)
        {
            Console.ForegroundColor = (ConsoleColor)TimeManagerColors.Title;
            Console.WriteLine(title.ToUpper());
            Console.ResetColor();
        }
        public static string Actions(FormFieldAction[] actions, bool margin = false)
        {
            foreach (FormFieldAction answer in actions)
            {
                if (margin)
                {
                    Console.Write("- ");
                }
                Console.Write("(");
                Console.ForegroundColor = (ConsoleColor)TimeManagerColors.ListDots;
                Console.Write(answer.Index);
                Console.ResetColor();
                Console.Write(") ");

                string end = "";
                Console.Write(answer.Title+end);
                
                if (answer.Description != null)
                {
                    end = ": ";
                    Console.Write(end);
                    Console.ForegroundColor = (ConsoleColor)TimeManagerColors.FormDescription;
                    Console.Write(answer.Description);
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
            return Console.ReadLine();
        }
        public static void ShowRecordingTasks(Dictionary<Proyect, List<Record>> orderedRecords, List<Task>tasks, out List<FormFieldAction>taskByAnswer)
        {
            taskByAnswer = new List<FormFieldAction>();
            if (orderedRecords.Count == 0)
            {
                Console.WriteLine("No proyects are being recorded...");
                return;
            }

            int proyectDotCounter = 0;
            int taskDotCounter = 0;

            foreach (var orderedRecord in orderedRecords)
            {
                Console.ForegroundColor = orderedRecord.Key.ProyectColor;
                string dot = UpperLetters[proyectDotCounter];
                Console.Write($"({dot}) {orderedRecord.Key.ProyectName}");

                foreach (var activeRecord in orderedRecord.Value)
                {
                    TimeSpan currentTime = DateTime.Now - activeRecord.RecordStart;
                    string subDot = LowerLetters[taskDotCounter];
                    string title = (activeRecord.IdTask>0)? tasks.Find(t=>t.IdTask==activeRecord.IdTask).TaskTitle : "NO";
                    if ( title == "NO")
                    {
                        Console.WriteLine($"-> {currentTime.ToString()}");
                        break;
                    }
                    Console.WriteLine();
                    Console.Write($"- ({subDot}) {title} -> {currentTime}");
                    taskDotCounter++;
                    taskByAnswer.Add(new FormFieldAction($"{dot}{subDot}", title, null, activeRecord.IdTask));
                }
                Console.WriteLine();
                taskByAnswer.Add(new FormFieldAction($"{dot}", orderedRecord.Key.ProyectName, null, orderedRecord.Key.IdProyect));
                proyectDotCounter++;
                taskDotCounter = 0;
            }

        }
        public static void ShowTask_Records(List<Task> tasks, ConsoleColor color, out List<FormFieldAction>tasksByIndex)
        {
            tasksByIndex = new List<FormFieldAction>();
            for (int i = 0; i < tasks.Count; i++)
            {
                Console.ForegroundColor = color;
                Console.Write($"({UpperLetters[i]}) {tasks[i].TaskTitle}");
                Console.ResetColor();
                Description($": {tasks[i].TaskDescription}");
                tasksByIndex.Add(new FormFieldAction(UpperLetters[i], tasks[i].TaskTitle, tasks[i].TaskDescription, tasks[i].IdTask));
            }
        }
        // FIXME It Object reference not set to an instance of an object at BetterConsoleTables.Table.GetColumnLengths()
        public static void ShowReport(List<ReportTableRow> table, string title)
        {
            List<ColumnHeader> headers = new List<ColumnHeader> {
                new ColumnHeader("Title", Alignment.Center, Alignment.Center),
                new ColumnHeader("Description", Alignment.Left, Alignment.Center),
                new ColumnHeader("Proyect Color", Alignment.Center, Alignment.Center),
                new ColumnHeader("Recorded Time", Alignment.Center, Alignment.Center)
            };

            Table reportTable = new Table(headers.ToArray());
            foreach (var item in table)
            {
                reportTable.AddRow(item.Title, item.Description, item.ProyectColor, item.RecordedTime);
            }

            Console.Clear();
            CreateTitle($"{title} REPORT");
            if (table.Count != 0)
            {
                reportTable.Config = TableConfiguration.Markdown();
                Console.WriteLine(reportTable.ToString());
            } else
            {
                Error("There are no proyects for making a report.");
                Console.WriteLine("Please go back to the main menú and create one.");
            }
            Description("Press enter to go back.");
            Console.ReadLine();
        }
        public static Dictionary<string, string> Form(Form form )
        {
            Dictionary<string, string> answers = new Dictionary<string, string>();

            Console.ForegroundColor = (ConsoleColor)TimeManagerColors.FormDescription;
            Console.WriteLine("Respond with an x to cancel the form.");
            Console.ResetColor();

            for (int i = 0; i < form.Fields.Length; i++)
            {
                FormField field = form.Fields[i];

                string required = (field.Required) ? RequiredSymbol : "";
                Console.ForegroundColor = (ConsoleColor)TimeManagerColors.FormField;
                Console.Write($"{field.Name}{required}:");
                Console.ResetColor();

                string answer = "";
                if (field.Type.Equals(FormFieldType.Radio))
                {
                    Console.WriteLine();
                    answer = Actions(field.PosibleAnswers.ToArray());
                }
                else if (field.Type.Equals(FormFieldType.Password))
                {
                    while (true)
                    {
                        var key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Enter)
                        {
                            break;
                        }
                        answer += key.KeyChar;
                    }
                }
                else
                {
                    answer = Console.ReadLine();
                }
                
                if (answer == "x") { return new Dictionary<string, string>(); }
                if (answer.Length == 0) { answer = null; }
                if(!form.Validate(ref answer, field))
                {
                    Error(form.Error);
                    i--;
                    continue;
                }
                answers.Add(field.Name, answer);
            }
            return answers;
        }
    }
}
