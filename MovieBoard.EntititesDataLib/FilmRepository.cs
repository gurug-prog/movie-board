using System;
using System.Collections.Generic;
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

        public int GetCount()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM films";
            int count = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            return count;
        }

        public Film GetFilm(SqliteDataReader reader)
        {
            var film = new Film();
            film.id = int.Parse(reader.GetString(0));
            film.title = reader.GetString(1);
            film.director = reader.GetString(2);
            film.country = reader.GetString(3);
            film.releaseYear = int.Parse(reader.GetString(4));
            film.duration = TimeSpan.Parse(reader.GetString(5));
            return film;
        }

        public int GetMaxId()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT max(id) FROM films";
            var queryResult = command.ExecuteScalar();
            if (queryResult is DBNull)
            {
                return 0;
            }
            int maxId = Convert.ToInt32(queryResult);
            connection.Close();
            return maxId;
        }

        public List<Film> GetPage(int pageNumber)
        {
            if (pageNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNumber));
            }
            connection.Open();
            var pageLength = 10;
            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"SELECT * FROM films LIMIT 10 OFFSET $offset";
            command.Parameters.AddWithValue("$offset", (pageNumber - 1) * pageLength);

            var reader = command.ExecuteReader();
            var pageFilms = new List<Film>();

            while (reader.Read())
            {
                pageFilms.Add(GetFilm(reader));
            }

            reader.Close();
            connection.Close();
            return pageFilms;
        }

        public int GetTotalPages()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM films";
            int count = Convert.ToInt32(command.ExecuteScalar());
            var pagesQuantity = (int)Math.Ceiling(count / 10.0);
            connection.Close();
            return pagesQuantity;
        }

        public int GetSearchPagesCount(string searchValue)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM films WHERE title LIKE '%' || $value || '%'";
            command.Parameters.AddWithValue("$value", searchValue);
            int count = Convert.ToInt32(command.ExecuteScalar());
            var pagesCount = (int)Math.Ceiling(count / 10.0);
            connection.Close();
            return pagesCount;
        }

        public List<Film> GetSearchPages(string searchValue, int page)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            var pageLength = 10;
            command.CommandText = @"SELECT * FROM films WHERE title LIKE '%'
                                    || $value || '%' LIMIT 10 OFFSET $offset";
            command.Parameters.AddWithValue("$offset", (page - 1) * pageLength);
            command.Parameters.AddWithValue("$value", searchValue);

            SqliteDataReader reader = command.ExecuteReader();
            List<Film> films = new List<Film>();
            while (reader.Read())
            {
                var film = GetFilm(reader);
                films.Add(film);
            }
            reader.Close();
            connection.Close();
            return films;
        }
    }
}