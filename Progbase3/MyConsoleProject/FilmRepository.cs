using System;
using Microsoft.Data.Sqlite;

namespace MyConsoleProject
{
    public class FilmRepository
    {
        private SqliteConnection connection;

        public FilmRepository(SqliteConnection connection)
        {
            this.connection = connection;
        }

        public int Insert(Film film)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"
                INSERT INTO films (title, director, country, releaseYear, duration)
                VALUES ($title, $director, $country, $releaseYear, $duration);

                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$title", film.title);
            command.Parameters.AddWithValue("$director", film.director);
            command.Parameters.AddWithValue("$country", film.country);
            command.Parameters.AddWithValue("$releaseYear", film.releaseYear);
            command.Parameters.AddWithValue("$duration", film.duration.ToString());

            var newId = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            return newId;
        }

        public Film GetById(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM films WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            SqliteDataReader reader = command.ExecuteReader();
            var film = new Film();
            if (reader.Read())
            {
                film.id = int.Parse(reader.GetString(0));
                film.title = reader.GetString(1);
                film.director = reader.GetString(2);
                film.country = reader.GetString(3);
                film.releaseYear = int.Parse(reader.GetString(4));
                film.duration = TimeSpan.Parse(reader.GetString(5));
            }
            else
            {
                film = null;
            }
            reader.Close();
            connection.Close();
            return film;
        }

        public bool Update(int id, Film film)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"UPDATE films SET title = $title, director = $director,
            country = $country, releaseYear = $releaseYear,
            duration = $duration WHERE id = $id";
            command.Parameters.AddWithValue("$title", film.title);
            command.Parameters.AddWithValue("$director", film.director);
            command.Parameters.AddWithValue("$country", film.country);
            command.Parameters.AddWithValue("$releaseYear", film.releaseYear);
            command.Parameters.AddWithValue("$duration", film.duration.ToString());

            var nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged == 1;
        }

        public bool DeleteById(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM films WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            var nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged == 1;
        }
    }
}