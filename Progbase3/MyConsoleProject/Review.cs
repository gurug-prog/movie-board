using System;

namespace MyConsoleProject
{
    public class Review
    {
        public int id;
        public string header; // textFile with random headers
        public string overview; // random comment
        public int rating; // in range from 1 to 10 (round to first digit after comma) ??????
                           // or ban to input user non-one afretcomma digit
        public DateTime lastEdited; // DateTime.Now

        public Review()
        {
            id = 0;
            header = "";
            overview = "";
            rating = 0;
            lastEdited = new DateTime();
        }

        public Review(string header, string overview, int rating, DateTime lastEdited)
        {
            this.header = header;
            this.overview = overview;
            this.rating = rating;
            this.lastEdited = lastEdited;
        }
    }
}