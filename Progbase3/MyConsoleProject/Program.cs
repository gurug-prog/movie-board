using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.IO;

namespace MyConsoleProject
{
    class Program
    {
        static string GenerateLogin()
        {
            char symbol;
            var login = "";
            var rand = new Random();
            for (int i = 0; i < rand.Next(3, 20); i++)
            {
                symbol = (char)rand.Next(97, 123);
                login += symbol;
            }
            for (int i = 0; i < rand.Next(0, 5); i++)
            {
                symbol = (char)rand.Next(48, 58);
                login += symbol;
            }
            return login;
        }

        static string GeneratePassword()
        {
            char symbol;
            var password = "";
            var rand = new Random();
            for (int i = 0; i < rand.Next(8, 15); i++)
            {
                symbol = (char)rand.Next(33, 127);
                password += symbol.ToString();
            }
            return password;
        }

        static DateTime GenerateDate(DateTime startDate, DateTime endDate)
        {
            var rand = new Random();
            var dateInterval = endDate - startDate;
            var hours = rand.Next(0, 24);
            var minutes = rand.Next(0, 60);
            var seconds = rand.Next(0, 60);
            var value = rand.Next(dateInterval.Days);
            var dt = startDate.AddDays(value).AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);
            return dt;
        }

        static TimeSpan GenerateDuration(TimeSpan minDuration, TimeSpan maxDuration)
        {
            var rand = new Random();
            var hours = rand.Next(minDuration.Hours, maxDuration.Hours + 1);
            
            var minMinutes = Math.Min(minDuration.Minutes, maxDuration.Minutes);
            var maxMinutes = minMinutes == minDuration.Minutes ? maxDuration.Minutes : minDuration.Minutes;
            var minutes = rand.Next(minMinutes, maxMinutes + 1);
            
            var minSeconds = Math.Min(minDuration.Seconds, maxDuration.Seconds);
            var maxSeconds = minSeconds == minDuration.Seconds ? maxDuration.Seconds : minDuration.Seconds;
            var seconds = rand.Next(minSeconds, maxSeconds + 1);

            return new TimeSpan(hours, minutes, seconds);
        }

        static List<User> GenerateUsers(int usersQuantity, DateTime startDate, DateTime endDate)
        {
            var users = new List<User>();
            var rand = new Random();
            var roles = new string[] { "moderator", "author" };

            for (int i = 0; i < usersQuantity; i++)
            {
                var login = GenerateLogin();
                var password = GeneratePassword();
                var role = roles[rand.Next(0, 2)];
                var signUpDate = GenerateDate(startDate, endDate);

                var user = new User(login, password, role, signUpDate);
                users.Add(user);
            }
            return users;
        }

        static List<Film> GenerateFilms(int filmsQuantity, int startYear,
            int endYear, TimeSpan minDuration, TimeSpan maxDuration)
        {
            var films = new List<Film>();
            var rand = new Random();
            var filmsFilePath = "../../data/generator/films.csv";
            var filmsLines = File.ReadAllLines(filmsFilePath);

            for (int i = 0; i < filmsQuantity; i++)
            {
                var filmsValues = filmsLines[i % filmsLines.Length].Split(',');

                var title = filmsValues[1];
                var director = filmsValues[2];
                var country = filmsValues[3];
                var releaseYear = rand.Next(startYear, endYear + 1);
                var duration = GenerateDuration(minDuration, maxDuration);

                var film = new Film(title, releaseYear, director, country, duration);
                films.Add(film);
            }
            return films;
        }

        static List<Actor> GenerateActors(int actorsQuantity, int minAge, int maxAge)
        {
            var actors = new List<Actor>();
            var rand = new Random();
            var actorsFilePath = "../../data/generator/actors.csv";
            var actorsLines = File.ReadAllLines(actorsFilePath);
            var rolePlans = new string[]{ "leading", "supporting", "extra" };

            for (int i = 0; i < actorsQuantity; i++)
            {
                var actorsValues = actorsLines[i % actorsLines.Length].Split(',');

                var fullName = actorsValues[1] + " " + actorsValues[2];
                var age = rand.Next(minAge, maxAge + 1);
                var rolePlan = rolePlans[rand.Next(0, 2)];

                var actor = new Actor(fullName, age, rolePlan);
                actors.Add(actor);
            }
            return actors;
        }

        static List<Review> GenerateReviews(int reviewsQuantity, DateTime startDate, DateTime endDate)
        {
            var reviews = new List<Review>();
            var rand = new Random();
            var headersFilePath = "../../data/generator/review_headers.txt";
            var overviewsFilePath = "../../data/generator/review_overviews.txt";
            var headers = File.ReadAllLines(headersFilePath);
            var overviews = File.ReadAllLines(overviewsFilePath);

            for (int i = 0; i < reviewsQuantity; i++)
            {
                var header = headers[i % headers.Length];
                var overview = overviews[i % overviews.Length];
                var rating = rand.Next(1, 11);
                var lastEdited = GenerateDate(startDate, endDate);

                var review = new Review(header, overview, rating, lastEdited);
                reviews.Add(review);
            }
            return reviews;
        }

        static void ProcessWriteUsers(string[] args, int entitiesQuantity, SqliteConnection connection)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("You have entered more than needed arguments.");
                return;
            }
            var dateIntervalString = args[2];
            var dateBoundsValues = dateIntervalString.Split('-');
            if (dateBoundsValues.Length != 2)
            {
                Console.WriteLine("You have entered wrong date interval.");
                return;
            }

            DateTime startDate, endDate;
            var startDateIsNotCorrect = !DateTime.TryParse(dateBoundsValues[0], out startDate);
            var endDateIsNotCorrect = !DateTime.TryParse(dateBoundsValues[1], out endDate);
            if (startDateIsNotCorrect || endDateIsNotCorrect || startDate > endDate)
            {
                Console.WriteLine("Dates that you have entered are not correct.");
                return;
            }

            var users = GenerateUsers(entitiesQuantity, startDate, endDate);
            var userRepo = new UserRepository(connection);
            foreach (var user in users)
            {
                userRepo.Insert(user);
            }
        }

        static void ProcessWriteFilms(string[] args, int entitiesQuantity, SqliteConnection connection)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("You have entered more than needed arguments.");
                return;
            }
            var yearsInterval = args[2];
            var yearsBoundsValues = yearsInterval.Split('-');
            if (yearsBoundsValues.Length != 2)
            {
                Console.WriteLine("You have entered wrong years interval.");
                return;
            }

            int startYear, endYear;
            var startYearIsNotCorrect = !int.TryParse(yearsBoundsValues[0], out startYear) || startYear < 0;
            var endYearIsNotCorrect = !int.TryParse(yearsBoundsValues[1], out endYear) || endYear < 0;
            if (startYearIsNotCorrect || endYearIsNotCorrect || startYear > endYear)
            {
                Console.WriteLine("Years must be non-negative integer numbers,");
                Console.WriteLine("such that startYear <= endYear.");
                return;
            }

            var durationInterval = args[3];
            var durationBoundsValues = durationInterval.Split('-');
            if (durationBoundsValues.Length != 2)
            {
                Console.WriteLine("You have entered wrong durations interval.");
                return;
            }

            TimeSpan minDuration, maxDuration;
            var minDurationIsNotCorrect = !TimeSpan.TryParse(durationBoundsValues[0], out minDuration);
            var maxDurationIsNotCorrect = !TimeSpan.TryParse(durationBoundsValues[1], out maxDuration);
            if (minDurationIsNotCorrect || maxDurationIsNotCorrect || minDuration > maxDuration)
            {
                Console.WriteLine("Durations that you have entered are not correct.");
                return;
            }

            var films = GenerateFilms(entitiesQuantity, startYear,
                endYear, minDuration, maxDuration);
            var filmRepo = new FilmRepository(connection);
            foreach (var film in films)
            {
                filmRepo.Insert(film);
            }
        }

        static void ProcessWriteActors(string[] args, int entitiesQuantity, SqliteConnection connection)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("You have entered more than needed arguments.");
                return;
            }
            var agesInterval = args[2];
            var agesBoundsValues = agesInterval.Split('-');
            if (agesBoundsValues.Length != 2)
            {
                Console.WriteLine("You have entered wrong ages interval.");
                return;
            }

            int minAge, maxAge;
            var minAgeIsNotCorrect = !int.TryParse(agesBoundsValues[0], out minAge) || minAge < 0;
            var maxAgeIsNotCorrect = !int.TryParse(agesBoundsValues[1], out maxAge) || maxAge < 0;
            if (minAgeIsNotCorrect || maxAgeIsNotCorrect || minAge > maxAge)
            {
                Console.WriteLine("Ages must be non-negative integer numbers,");
                Console.WriteLine("such that minAge <= maxAge.");
                return;
            }

            var actors = GenerateActors(entitiesQuantity, minAge, maxAge);
            var actorRepo = new ActorRepository(connection);
            foreach (var actor in actors)
            {
                actorRepo.Insert(actor);
            }
        }

        static void ProcessWriteReviews(string[] args, int entitiesQuantity, SqliteConnection connection)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("You have entered more than needed arguments.");
                return;
            }
            var dateIntervalString = args[2];
            var dateBoundsValues = dateIntervalString.Split('-');
            if (dateBoundsValues.Length != 2)
            {
                Console.WriteLine("You have entered wrong date interval.");
                return;
            }

            DateTime startDate, endDate;
            var startDateIsNotCorrect = !DateTime.TryParse(dateBoundsValues[0], out startDate);
            var endDateIsNotCorrect = !DateTime.TryParse(dateBoundsValues[1], out endDate);
            if (startDateIsNotCorrect || endDateIsNotCorrect || startDate > endDate)
            {
                Console.WriteLine("Dates that you have entered are not correct.");
                return;
            }

            var reviews = GenerateReviews(entitiesQuantity, startDate, endDate);
            var reviewRepo = new ReviewRepository(connection);
            foreach (var review in reviews)
            {
                reviewRepo.Insert(review);
            }
        }

        static void Main(string[] args)
        {
            // args = new string[] { "", "", "", "" };
            var databaseFilePath = "../../data/database.db";
            SqliteConnection connection = new SqliteConnection($"Data Source={databaseFilePath}");

            // this conditional needed when user don't wanna generate entities
            if (args.Length == 0)
            {
                Console.WriteLine("Hello World!");
                return;
            }

            if (args.Length < 3)
            {
                Console.WriteLine("You have entered less than needed command arguments.");
                return;
            }
            var entitiesQuantity = 0;
            var entitiesQuantityIsNotIntegerOrNegativeNumber =
                !int.TryParse(args[1], out entitiesQuantity) || entitiesQuantity < 0;
            if (entitiesQuantityIsNotIntegerOrNegativeNumber)
            {
                Console.WriteLine("Quantity of entities must be positive integer number.");
                return;
            }

            if (args[0] == "user")
            {
                ProcessWriteUsers(args, entitiesQuantity, connection);
            }
            else if (args[0] == "film")
            {
                ProcessWriteFilms(args, entitiesQuantity, connection);
            }
            else if (args[0] == "actor")
            {
                ProcessWriteActors(args, entitiesQuantity, connection);
            }
            else if (args[0] == "review")
            {
                ProcessWriteReviews(args, entitiesQuantity, connection);
            }
            else
            {
                Console.WriteLine($"Unknown entity: {args[0]}");
                return;
            }
        }
    }
}