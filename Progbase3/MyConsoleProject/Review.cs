using System;

namespace MyConsoleProject
{
    public class Review
    {
        public int id;
        public string header; // textFile with random headers
        public string overview; // random comment
        public DateTime lastEdited; // DateTime.Now
        public int rating; // in range from 1 to 10 (round to first digit after comma) ??????
                               // or ban to input user non-one afretcomma digit
    }
}