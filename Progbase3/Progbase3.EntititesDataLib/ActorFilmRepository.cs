using System;
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