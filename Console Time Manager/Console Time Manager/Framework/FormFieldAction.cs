using System;
using System.Collections.Generic;
using System.Text;

namespace Console_Time_Manager.Framework
{
    class FormFieldAction
    {
        public string Index { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public long Id { get; private set; }
        public FormFieldAction(string index, string title, string description = null, long id = 0)
        {
            Index = index;
            Title = title;
            Description = description;
            Id = id;
        }
    }
}
