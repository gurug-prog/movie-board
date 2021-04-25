namespace MyConsoleProject
{
    public class Actor
    {
        public int id;
        public string fullName; // textFile
        public int age; // int in range 15 - 80
        public string rolePlan; // leading, supporting, extra

        public Actor()
        {
            id = 0;
            fullName = "";
            age = 0;
            rolePlan = "";
        }

        public Actor(int id, string fullName, byte age, string rolePlan)
        {
            this.id = id;
            this.fullName = fullName;
            this.age = age;
            this.rolePlan = rolePlan;
        }
    }
}