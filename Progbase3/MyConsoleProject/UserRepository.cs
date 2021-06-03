using System;
using System.Collections.Generic;
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

        public List<User> GetSearchPages(string searchValue, int page)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            var pageLength = 10;
            command.CommandText = @"SELECT * FROM users WHERE login LIKE '%'
                                    || $value || '%' LIMIT 10 OFFSET $offset";
            command.Parameters.AddWithValue("$offset", (page - 1) * pageLength);
            command.Parameters.AddWithValue("$value", searchValue);

            SqliteDataReader reader = command.ExecuteReader();
            // List<User> users = null;
            List<User> users = new List<User>();
            while (reader.Read())
            {
                var user = GetUser(reader);
                users.Add(user);
            }
            ////////////
            ////////////
            ////////////
            // if (users.Count == 0)
            // {
            //     users.Add(new User());
            // }
            reader.Close();
            connection.Close();
            return users;
        }

        public int GetSearchPagesCount(string searchValue)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM users WHERE login LIKE '%' || $value || '%'";
            command.Parameters.AddWithValue("$value", searchValue);
            int count = Convert.ToInt32(command.ExecuteScalar());
            var pagesCount = (int)Math.Ceiling(count / 10.0);
            ////////////
            ////////////
            ////////////
            // if (pagesCount == 0)
            // {
            //     pagesCount++;
            // }
            connection.Close();
            return pagesCount;
        }

        public User GetUser(SqliteDataReader reader)
        {
            var user = new User();
            user.id = int.Parse(reader.GetString(0));
            user.login = reader.GetString(1);
            user.password = reader.GetString(2);
            user.role = reader.GetString(3);
            user.signUpDate = DateTime.Parse(reader.GetString(4));
            return user;
        }

        // public List<User> GetAll()
        // {
        //     connection.Open();
        //     var command = connection.CreateCommand();
        //     command.CommandText = @"SELECT * FROM users";
        //     var reader = command.ExecuteReader();
        //     var users = new List<User>();

        //     while (reader.Read())
        //     {
        //         users.Add(GetUser(reader));
        //     }

        //     reader.Close();
        //     connection.Close();
        //     return users;
        // }

        public int GetTotalPages()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM users";
            int count = Convert.ToInt32(command.ExecuteScalar());
            var pagesQuantity = (int)Math.Ceiling(count / 10.0);
            connection.Close();
            return pagesQuantity;
        }

        public List<User> GetPage(int pageNumber)
        {
            if (pageNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNumber));
            }
            connection.Open();
            var pageLength = 10;
            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"SELECT * FROM users LIMIT 10 OFFSET $offset";
            command.Parameters.AddWithValue("$offset", (pageNumber - 1) * pageLength);

            var reader = command.ExecuteReader();
            var pageUsers = new List<User>();

            while (reader.Read())
            {
                pageUsers.Add(GetUser(reader));
            }

            reader.Close();
            connection.Close();
            return pageUsers;
        }
    }
}