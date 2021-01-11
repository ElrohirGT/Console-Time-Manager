using System;
using System.Collections.Generic;
using System.Text;

namespace Console_Time_Manager.Framework
{
    class Form
    {
        public FormField[] Fields { get; private set; }
        public string Error { get; private set; }
        public Form(FormField[] fields)
        {
            Fields = fields;
        }
        public bool Validate(ref string answer, FormField field)
        {
            Error = field.Validation(ref answer);
            return (Error.Length != 0) ? false : true;
        }
        public FormField GetField(string fieldName)
        {
            foreach (FormField listField in Fields)
            {
                if (listField.Name == fieldName)
                {
                    return listField;
                }
            }
            return new FormField("");
        }
    }
}
