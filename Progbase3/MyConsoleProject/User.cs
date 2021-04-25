using System;
using System.Collections.Generic;

namespace MyConsoleProject
{
    public class User
    {
        private int id;
        private string login; // textFile with logins
        private string password; // random string (generator method)
        private string role; // moderator or author or admin
        private DateTime registrationDate; // DateTime.Now
    }
}