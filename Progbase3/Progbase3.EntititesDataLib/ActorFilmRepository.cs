using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace MyConsoleProject
{
    public class ActorFilmRepository
    {
        private SqliteConnection connection;

        public ActorFilmRepository(SqliteConnection connection)
        {
            this.connection = connection;
        }

        public int Insert(ActorFilm actorFilm)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"
                INSERT INTO actors_films (actorId, filmId)
                VALUES ($actorId, $filmId);

                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$actorId", actorFilm.actorId);
            command.Parameters.AddWithValue("$filmId", actorFilm.filmId);

            var newId = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            return newId;
        }

        public ActorFilm GetById(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM actors_films WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            SqliteDataReader reader = command.ExecuteReader();
            var actorFilm = new ActorFilm();
            if (reader.Read())
            {
                actorFilm.id = int.Parse(reader.GetString(0));
                actorFilm.actorId = int.Parse(reader.GetString(1));
                actorFilm.filmId = int.Parse(reader.GetString(2));
            }
            else
            {
                actorFilm = null;
            }
            reader.Close();
            connection.Close();
            return actorFilm;
        }

        ////////////////////
        ////////////////////
        ////////////////////
        ////////////////////
        ////////////////////
        public bool Update(int id, ActorFilm actorFilm)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"UPDATE actors_films SET actorId = $actorId,
            filmId = $filmId, WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            command.Parameters.AddWithValue("$actorId", actorFilm.actorId);
            command.Parameters.AddWithValue("$filmId", actorFilm.filmId);

            var nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged == 1;
        }

        public bool DeleteById(int id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM actors_films WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            var nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged == 1;
        }

        public bool GetRelationExistence(int actorId, int filmId)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM actors_films WHERE
                                    actorId = $actorId AND filmId = $filmId";
            command.Parameters.AddWithValue("$actorId", actorId);
            command.Parameters.AddWithValue("$filmId", filmId);
            int countRelations = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();

            if (countRelations == 1)
            {
                return true;
            }
            return false;
        }
    }
}