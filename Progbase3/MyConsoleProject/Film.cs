using System;

namespace MyConsoleProject
{
    public class Film
    {
        private int id;
        private string title; // random title (file)
        private string director; // random film directors (file)
        private string country; // random country (file)
        private int releaseYear; // random int from 1950 - 2021
        private TimeSpan duration; // random duration from (00, 15, 29) to (05, 59, 59)
                                   // hours from 0 to 5
                                   // minutes from 0 to 59
                                   // if hours == 0 -> minutes from 3 to 59
                                   // seconds from 0 to 59
        // public Film(string title, int releaseYear, string director, string country, TimeSpan duration)
        // {
        //     this.title = title;
        //     this.releaseYear = releaseYear;
        //     this.director = director;
        //     this.country = country;
        //     this.duration = duration;
        // }
    }
}