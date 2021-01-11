using System;
using System.Collections.Generic;
using System.Text;

namespace Console_Time_Manager.Models
{
    public class User
    {
        public long IdUser { get; private set; }
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public void Populate(string un, string email, string pwd)
        {
            UserName = un;
            Email = email;
            Password = pwd;
        }
        public void SetId(long id)
        {
            IdUser = id;
        }
    }
}
