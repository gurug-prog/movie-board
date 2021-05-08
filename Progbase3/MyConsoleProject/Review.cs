using System;

namespace MyConsoleProject
{
    public class Review
    {
        public int id;
        public string header;
        public string overview;
        public int rating;
        public DateTime lastEdited; // DateTime.Now
        // public Film film;
        public int filmId;
        // public User author;
        public int userId;

        public Review()
        {
            id = 0;
            header = "";
            overview = "";
            rating = 0;
            lastEdited = new DateTime();
        }

        // public Review(string header, string overview, int rating, DateTime lastEdited)
        // {
        //     this.header = header;
        //     this.overview = overview;
        //     this.rating = rating;
        //     this.lastEdited = lastEdited;
        // }

        public Review(string header, string overview, int rating, DateTime lastEdited, int filmId, int userId)
        {
            this.header = header;
            this.overview = overview;
            this.rating = rating;
            this.lastEdited = lastEdited;
            this.filmId = filmId;
            this.userId = userId;
        }
    }
}