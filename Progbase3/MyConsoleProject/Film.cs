using System;
using System.Collections.Generic;

namespace MyConsoleProject
{
    public class Film
    {
        public int id;
        public string title;
        public string director;
        public string country;
        public int releaseYear;
        public TimeSpan duration;
        public List<Review> reviews;
        public List<Actor> cast;

        public Film()
        {
            id = 0;
            title = "";
            director = "";
            country = "";
            releaseYear = 1900;
            duration = new TimeSpan();
        }

        public Film(string title, int releaseYear, string director, string country, TimeSpan duration)
        {
            this.title = title;
            this.releaseYear = releaseYear;
            this.director = director;
            this.country = country;
            this.duration = duration;
        }
    }
}