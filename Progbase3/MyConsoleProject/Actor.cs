namespace MyConsoleProject
{
    public class Actor
    {
        public int id;
        public string fullName; // textFile
        public byte age; // int in range 15 - 80
        public string rolePlan; // leading, supporting, extra

        public Actor(int id, string fullName, byte age, string rolePlan)
        {
            this.id = id;
            this.fullName = fullName;
            this.age = age;
            this.rolePlan = rolePlan;
        }
    }
}