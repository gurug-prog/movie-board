using System;
using System.Collections.Generic;

namespace MyConsoleProject
{
    public class Actor
    {
        public int id;
        public string fullName;
        public int age;
        public string rolePlan;
        public List<Film> films;

        public Actor()
        {
            id = 0;
            fullName = "";
            age = 0;
            rolePlan = "";
        }

        public Actor(string fullName, int age, string rolePlan)
        {
            this.fullName = fullName;
            this.age = age;
            this.rolePlan = rolePlan;
        }

        public Actor(int id, string fullName, int age, string rolePlan)
        {
            this.id = id;
            this.fullName = fullName;
            this.age = age;
            this.rolePlan = rolePlan;
        }
    }
}