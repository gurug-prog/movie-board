using System;
using Microsoft.Data.Sqlite;

namespace MyConsoleProject
{
    public class ReviewRepository
    {
        private SqliteConnection connection;

        public ReviewRepository(SqliteConnection connection)
        {
            this.connection = connection;
        }

        public int Insert(Review review)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"
                INSERT INTO reviews (header, overview, rating, lastEdited)
                VALUES ($header, $overview, $rating, $lastEdited);

                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$header", review.header);
            command.Parameters.AddWithValue("$overview", review.overview);
            command.Parameters.AddWithValue("$rating", review.rating);
            command.Parameters.AddWithValue("$lastEdited", review.lastEdited.ToString());

            var newId = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            return newId;
        }

        public Review GetById(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM reviews WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            SqliteDataReader reader = command.ExecuteReader();
            var review = new Review();
            if (reader.Read())
            {
                review.id = int.Parse(reader.GetString(0));
                review.header = reader.GetString(1);
                review.overview = reader.GetString(2);
                review.rating = int.Parse(reader.GetString(3));
                review.lastEdited = DateTime.Parse(reader.GetString(3));
            }
            else
            {
                review = null;
            }
            reader.Close();
            connection.Close();
            return review;
        }

        public bool Update(int id, Review review)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"UPDATE review SET header = $header, overview = $overview,
            rating = $rating, lastEdited = $lastEdited WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            command.Parameters.AddWithValue("$header", review.header);
            command.Parameters.AddWithValue("$overview", review.overview);
            command.Parameters.AddWithValue("$rating", review.rating);
            command.Parameters.AddWithValue("$lastEdited", review.lastEdited.ToString());

            var nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged == 1;
        }

        public bool DeleteById(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM reviews WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            var nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged == 1;
        }
    }
}