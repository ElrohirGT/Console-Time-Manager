using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Console_Time_Manager.Framework
{
    public enum FormFieldType
    {
        String = 0,
        Number = 1,
        Email = 2,
        Password = 3,
        Radio = 4,
    }
    class FormField
    {
        public string Name { get; private set; }
        public FormFieldType Type { get; private set; }
        public bool Required { get; private set; }
        public int MinimunLength { get; private set; }
        public int MaximunLength { get; private set; }
        public bool LogIn { get; private set; }
        public List<FormFieldAction> PosibleAnswers { get; private set; }
        public string DefaultAnswer { get; private set; }
        public FormField(string name, FormFieldType type = FormFieldType.String, bool required = false, bool isInLogIn = false, int minimunLength = 0, int maximunLength = 255, string defaultAnswer = "", List<string> posibleAnswers = null)
        {
            Name = name;
            Type = type;
            Required = required;
            MinimunLength = minimunLength;
            MaximunLength = maximunLength;
            PosibleAnswers = CreateAnswers(posibleAnswers);
            LogIn = isInLogIn;
            DefaultAnswer = defaultAnswer;
        }
        private List<FormFieldAction> CreateAnswers(List<string> answers)
        {
            if (answers != null)
            {
                List<FormFieldAction> pAnswers = new List<FormFieldAction>();

                for (int i = 0; i < answers?.Count; i++)
                {
                    pAnswers.Add(new FormFieldAction($"{i}", answers[i]));
                }

                return pAnswers;
            }
            return null;
        }
        public string Validation(ref string answer)
        {
            string error = "";
            answer = (answer == null) ? DefaultAnswer : answer;
            string index = answer;
            if (Required && answer == null)
            {
                error += "This field is required!\n";
            }
            if (PosibleAnswers != null && PosibleAnswers.FindIndex((FormFieldAction ffa) => ffa.Index == index) == -1)
            {
                error += "The answer must be one of the above.\n";
            }
            if (Type == FormFieldType.String)
            {
                error += ValidateString(answer);
            }
            else if (Type == FormFieldType.Number)
            {
                error += ValidateNumber(answer);
            }
            else if (Type == FormFieldType.Email)
            {
                error += ValidateEmail(answer);
            }

            else if (Type == FormFieldType.Password)
            {
                error += ValidatePassword(answer);
            }
            return error;
        }
        private string ValidateString(string answer)
        {
            string error = "";
            if (decimal.TryParse(answer, out _))
            {
                error += "The answer must not be a number!\n";
            }
            if (answer.Length < MinimunLength)
            {
                error += $"Please write a longer answer! (minimun is {MinimunLength})\n";
            }
            if (answer.Length > MaximunLength)
            {
                error += $"Please write a shorter answer! (maximun is {MaximunLength})\n";
            }
            return error;
        }
        private string ValidateNumber(string answer)
        {
            string error = "";
            if (!decimal.TryParse(answer, out decimal number))
            {
                error += "The answer must be a number!\n";
            }
            if (number < MinimunLength)
            {
                error += $"Please write a longer answer! (minimun is {MinimunLength})\n";
            }
            if (number > MaximunLength)
            {
                error += $"Please write a shorter answer! (maximun is {MaximunLength})\n";
            }
            return error;
        }
        private string ValidateEmail(string answer)
        {
            string error = "";
            if (decimal.TryParse(answer, out decimal number))
            {
                error += "The answer must not be a number!\n";
            }
            if (!Regex.IsMatch(answer, @".*@.*\.com"))
            {
                error += "The answer must be a valid email!\n";
            }
            if (number < MinimunLength)
            {
                error += $"Please write a longer answer! (minimun is {MinimunLength})\n";
            }
            if (number > MaximunLength)
            {
                error += $"Please write a shorter answer! (maximun is {MaximunLength})\n";
            }
            return error;
        }
        private string ValidatePassword(string answer)
        {
            string error = "";
            if (decimal.TryParse(answer, out decimal _))
            {
                error += "The answer must not be a number!\n";
            }
            if (LogIn && !Regex.IsMatch(answer, @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*_-]).*$"))
            {
                error += "The password is too weak! Please remember to use special characters, numbers and capital letters.\n";
            }
            if (answer.Length < MinimunLength)
            {
                error += $"Please write a longer answer! (minimun is {MinimunLength})\n";
            }
            if (answer.Length > MaximunLength)
            {
                error += $"Please write a shorter answer! (maximun is {MaximunLength})\n";
            }
            return error;
        }
    }
}
