using System;
using Microsoft.Data.Sqlite;

namespace MyConsoleProject
{
    public class UserRepository
    {
        private SqliteConnection connection;

        public UserRepository(SqliteConnection connection)
        {
            this.connection = connection;
        }

        public int Insert(User user)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"
                INSERT INTO users (login, password, role, signUpDate)
                VALUES ($login, $password, $role, $signUpDate);

                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$login", user.login);
            command.Parameters.AddWithValue("$password", user.password);
            command.Parameters.AddWithValue("$role", user.role);
            command.Parameters.AddWithValue("$signUpDate", user.signUpDate.ToString("o"));

            var newId = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            return newId;
        }

        public User GetById(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            SqliteDataReader reader = command.ExecuteReader();
            var review = new User();
            if (reader.Read())
            {
                review.id = int.Parse(reader.GetString(0));
                review.login = reader.GetString(1);
                review.password = reader.GetString(2);
                review.role = reader.GetString(3);
                review.signUpDate = DateTime.Parse(reader.GetString(4));
            }
            else
            {
                review = null;
            }
            reader.Close();
            connection.Close();
            return review;
        }

        public bool Update(int id, User user)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"UPDATE users SET login = $login, password = $password,
            role = $role, signUpDate = $signUpDate WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            command.Parameters.AddWithValue("$login", user.login);
            command.Parameters.AddWithValue("$password", user.password);
            command.Parameters.AddWithValue("$role", user.role);
            command.Parameters.AddWithValue("$signUpDate", user.signUpDate.ToString("o"));

            var nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged == 1;
        }

        public bool DeleteById(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM users WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            var nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged == 1;
        }

        public int GetCount()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM users";
            int count = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            return count;
        }

        public int GetMaxId()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT max(id) FROM users";
            var queryResult = command.ExecuteScalar();
            if (queryResult is DBNull)
            {
                return 0;
            }
            int maxId = Convert.ToInt32(queryResult);
            connection.Close();
            return maxId;
        }
    }
}