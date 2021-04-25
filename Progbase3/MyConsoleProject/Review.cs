using System;

namespace MyConsoleProject
{
    public class Review
    {
        private int id;
        private string header; // textFile with random headers
        private string comment; // random comment
        private DateTime lastEdited; // DateTime.Now
        private double rating; // in range from 1.0 to 10.0 (round to first digit after comma) ??????
                               // or ban to input user non-one afretcomma digit
    }
}