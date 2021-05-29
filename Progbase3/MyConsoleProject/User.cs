using System;
using System.Collections.Generic;

namespace MyConsoleProject
{
    public class User
    {
        public int id;
        public string login;
        public string password;
        public string role; // moderator or author or admin
        public DateTime signUpDate; // DateTime.Now
        public List<Review> reviews;

        public User()
        {
            id = 0;
            login = "";
            password = "";
            role = "";
            signUpDate = new DateTime();
        }

        public User(string login, string password, string role, DateTime signUpDate)
        {
            this.login = login;
            this.password = password;
            this.role = role;
            this.signUpDate = signUpDate;
        }

        public override string ToString()
        {
            return String.Format("{0, -6} {1, -15} {2}", "[" + id + "]", login, "(" + role + ")");
        }
    }
}