using System;
using Microsoft.Data.Sqlite;

namespace MyConsoleProject
{
    public class ActorRepository
    {
        private SqliteConnection connection;

        public ActorRepository(SqliteConnection connection)
        {
            this.connection = connection;
        }

        public int Insert(Actor actor)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"
                INSERT INTO actors (fullName, age, rolePlan)
                VALUES ($fullName, $age, $rolePlan);

                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$fullName", actor.fullName);
            command.Parameters.AddWithValue("$age", actor.age);
            command.Parameters.AddWithValue("$rolePlan", actor.rolePlan);

            var newId = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            return newId;
        }

        public Actor GetById(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM actors WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            SqliteDataReader reader = command.ExecuteReader();
            var actor = new Actor();
            if (reader.Read())
            {
                actor.id = int.Parse(reader.GetString(0));
                actor.fullName = reader.GetString(1);
                actor.age = int.Parse(reader.GetString(2));
                actor.rolePlan = reader.GetString(3);
            }
            else
            {
                actor = null;
            }
            reader.Close();
            connection.Close();
            return actor;
        }

        public bool Update(int id, Actor actor)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"UPDATE actors SET fullName = $fullName, age = $age,
            rolePlan = $rolePlan, WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            command.Parameters.AddWithValue("$fullName", actor.fullName);
            command.Parameters.AddWithValue("$age", actor.age);
            command.Parameters.AddWithValue("$rolePlan", actor.rolePlan);

            var nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged == 1;
        }

        public bool DeleteById(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM actors WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            var nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged == 1;
        }

        // private static Task GetTask(SqliteDataReader reader)
        // {
        //     var task = new Task();
        //     task.id = int.Parse(reader.GetString(0));
        //     task.topic = reader.GetString(1);
        //     task.description = reader.GetString(2);
        //     task.maxPoints = int.Parse(reader.GetString(3));
        //     task.isPublished = reader.GetString(4) == "True" ? true : false;
        //     task.createdAt = DateTime.Parse(reader.GetString(5));
        //     return task;
        // }

        // public int GetTotalPages()
        // {
        //     connection.Open();
        //     SqliteCommand command = connection.CreateCommand();
        //     command.CommandText = @"SELECT COUNT(*) FROM tasks";
        //     int count = Convert.ToInt32(command.ExecuteScalar());
        //     var pagesQuantity = (int)Math.Ceiling(count / 10.0);
        //     connection.Close();
        //     return pagesQuantity;
        // }

        // public List<Task> GetPage(int pageNumber)
        // {
        //     connection.Open();
        //     SqliteCommand command = connection.CreateCommand();
        //     command.CommandText =
        //     @"SELECT * FROM tasks LIMIT 10 OFFSET $offset";
        //     command.Parameters.AddWithValue("$offset", (pageNumber - 1) * 10);

        //     var reader = command.ExecuteReader();
        //     var pageTasks = new List<Task>();

        //     while (reader.Read())
        //     {
        //         pageTasks.Add(GetTask(reader));
        //     }

        //     reader.Close();
        //     connection.Close();
        //     return pageTasks;
        // }

        // public List<Task> GetAll()
        // {
        //     connection.Open();
        //     var command = connection.CreateCommand();
        //     command.CommandText = @"SELECT * FROM tasks";
        //     var reader = command.ExecuteReader();
        //     var tasks = new List<Task>();

        //     while (reader.Read())
        //     {
        //         tasks.Add(GetTask(reader));
        //     }

        //     reader.Close();
        //     connection.Close();
        //     return tasks;
        // }
    }
}