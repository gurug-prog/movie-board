using System;
using System.Xml.Serialization;

namespace MyConsoleProject
{
    public class Review
    {
        public int id;
        public string header;
        public string overview;
        public int rating;
        [XmlElement("lastedited")]
        public DateTime lastEdited; // DateTime.Now
        // public Film film;
        [XmlElement("filmid")]
        public int filmId;
        // public User author;
        [XmlElement("userid")]
        public int userId;

        public Review()
        {
            id = 0;
            header = "";
            overview = "";
            rating = 0;
            lastEdited = new DateTime();
        }

        public Review(int id, string header, string overview, int rating, DateTime lastEdited, int filmId, int userId)
        {
            this.id = id;
            this.header = header;
            this.overview = overview;
            this.rating = rating;
            this.lastEdited = lastEdited;
            this.filmId = filmId;
            this.userId = userId;
        }

        public Review(string header, string overview, int rating, DateTime lastEdited, int filmId, int userId)
        {
            this.header = header;
            this.overview = overview;
            this.rating = rating;
            this.lastEdited = lastEdited;
            this.filmId = filmId;
            this.userId = userId;
        }

        public override string ToString()
        {
            return String.Format("{0, -6} {1, -15} {2}", "[" + id + "]", header, "(" + rating + ")");
        }
    }
}