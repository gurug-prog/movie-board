using System;
using System.Collections.Generic;

namespace MyConsoleProject
{
    public class User
    {
        public int id;
        public string login; // textFile with logins
        public string password; // random string (generator method)
        public string role; // moderator or author or admin
        public DateTime signUpDate; // DateTime.Now
    }
}