using System;
using System.Collections.Generic;
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
                INSERT INTO reviews (header, overview, rating, lastEdited, userId, filmId)
                VALUES ($header, $overview, $rating, $lastEdited, $userId, $filmId);

                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$header", review.header);
            command.Parameters.AddWithValue("$overview", review.overview);
            command.Parameters.AddWithValue("$rating", review.rating);
            command.Parameters.AddWithValue("$lastEdited", review.lastEdited.ToString("o"));
            command.Parameters.AddWithValue("$userId", review.userId);
            command.Parameters.AddWithValue("$filmId", review.filmId);

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
                review.lastEdited = DateTime.Parse(reader.GetString(4));
                review.userId = int.Parse(reader.GetString(5));
                review.filmId = int.Parse(reader.GetString(6));
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
            @"UPDATE reviews SET header = $header, overview = $overview,
            rating = $rating, lastEdited = $lastEdited WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            command.Parameters.AddWithValue("$header", review.header);
            command.Parameters.AddWithValue("$overview", review.overview);
            command.Parameters.AddWithValue("$rating", review.rating);
            command.Parameters.AddWithValue("$lastEdited", review.lastEdited.ToString("o"));

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

        public int GetCount()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM reviews";
            int count = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();
            return count;
        }

        public bool GetRelationExistence(int userId, int filmId)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM reviews WHERE
                                    userId = $userId AND filmId = $filmId";
            command.Parameters.AddWithValue("$userId", userId);
            command.Parameters.AddWithValue("$filmId", filmId);
            int countRelations = Convert.ToInt32(command.ExecuteScalar());
            connection.Close();

            if (countRelations == 0)
            {
                return true;
            }
            return false;
        }

        public List<Review> GetByUserId(int userId)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM reviews WHERE userId = $userId";
            command.Parameters.AddWithValue("$userId", userId);
            SqliteDataReader reader = command.ExecuteReader();

            var userReviews = new List<Review>();
            while (reader.Read())
            {
                var id = int.Parse(reader.GetString(0));
                var header = reader.GetString(1);
                var overview = reader.GetString(2);
                var rating = int.Parse(reader.GetString(3));
                var lastEdited = DateTime.Parse(reader.GetString(4));
                var filmId = int.Parse(reader.GetString(6));

                var review = new Review(header, overview, rating, lastEdited, filmId, userId);
                userReviews.Add(review);
            }

            reader.Close();
            connection.Close();
            return userReviews;
        }


        /////////////////////
        /////////////////////
        /////////////////////
        /////////////////////
        /////////////////////
        /////////////////////
        /////////////////////
        /////////////////////
        // метод для екпорту
        public List<Review> GetByFilmId(int filmId)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM reviews WHERE filmId = $filmId";
            command.Parameters.AddWithValue("$filmId", filmId);
            SqliteDataReader reader = command.ExecuteReader();

            var filmReviews = new List<Review>();
            while (reader.Read())
            {
                var id = int.Parse(reader.GetString(0));
                var header = reader.GetString(1);
                var overview = reader.GetString(2);
                var rating = int.Parse(reader.GetString(3));
                var lastEdited = DateTime.Parse(reader.GetString(4));
                var userId = int.Parse(reader.GetString(5));

                var review = new Review(id, header, overview, rating, lastEdited, filmId, userId);
                filmReviews.Add(review);
            }

            reader.Close();
            connection.Close();
            return filmReviews;
        }

        public List<Review> GetSearchPages(string searchValue, int page)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            var pageLength = 10;
            command.CommandText = @"SELECT * FROM reviews WHERE header LIKE '%'
                                    || $value || '%' LIMIT 10 OFFSET $offset";
            command.Parameters.AddWithValue("$offset", (page - 1) * pageLength);
            command.Parameters.AddWithValue("$value", searchValue);

            SqliteDataReader reader = command.ExecuteReader();
            List<Review> reviews = new List<Review>();
            while (reader.Read())
            {
                var review = GetReview(reader);
                reviews.Add(review);
            }
            reader.Close();
            connection.Close();
            return reviews;
        }

        public int GetSearchPagesCount(string searchValue)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM reviews WHERE header LIKE '%' || $value || '%'";
            command.Parameters.AddWithValue("$value", searchValue);
            int count = Convert.ToInt32(command.ExecuteScalar());
            var pagesCount = (int)Math.Ceiling(count / 10.0);
            connection.Close();
            return pagesCount;
        }

        public int GetTotalPages()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM reviews";
            int count = Convert.ToInt32(command.ExecuteScalar());
            var pagesQuantity = (int)Math.Ceiling(count / 10.0);
            connection.Close();
            return pagesQuantity;
        }

        public List<Review> GetPage(int pageNumber)
        {
            if (pageNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNumber));
            }
            connection.Open();
            var pageLength = 10;
            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"SELECT * FROM reviews LIMIT 10 OFFSET $offset";
            command.Parameters.AddWithValue("$offset", (pageNumber - 1) * pageLength);

            var reader = command.ExecuteReader();
            var pageReviews = new List<Review>();

            while (reader.Read())
            {
                pageReviews.Add(GetReview(reader));
            }

            reader.Close();
            connection.Close();
            return pageReviews;
        }

        public Review GetReview(SqliteDataReader reader)
        {
            var review = new Review();
            var id = int.Parse(reader.GetString(0));
            var header = reader.GetString(1);
            var overview = reader.GetString(2);
            var rating = int.Parse(reader.GetString(3));
            var lastEdited = DateTime.Parse(reader.GetString(4));
            var userId = int.Parse(reader.GetString(5));
            var filmId = int.Parse(reader.GetString(6));
            return review;
        }
    }
}