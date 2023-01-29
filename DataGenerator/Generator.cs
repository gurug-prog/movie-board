using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using ScottPlot;

namespace MyConsoleProject
{
    public static class Generator
    {
        public static string GenerateLogin()
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

        public static string GeneratePassword()
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

        public static DateTime GenerateDate(DateTime startDate, DateTime endDate)
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

        public static void GenerateActorFilmRelations(int startIndex, int endIndex, SqliteConnection connection)
        {
            var rand = new Random();
            var actorFilmRepo = new ActorFilmRepository(connection);
            var filmRepo = new FilmRepository(connection);
            var actorRepo = new ActorRepository(connection);
            for (int i = startIndex; i < endIndex + 1; i++)
            {
                var filmsCount = rand.Next(1, filmRepo.GetCount() + 1);
                var actorFilm = new ActorFilm();

                actorFilm.actorId = i;
                var actorId = actorFilm.actorId;
                var actor = actorRepo.GetById(actorId);
                if (actor == null)
                {
                    continue;
                }

                for (int j = 0; j < filmsCount; j++)
                {
                    var filmId = rand.Next(1, filmRepo.GetMaxId() + 1);
                    actorFilm.filmId = filmId;
                    var film = filmRepo.GetById(filmId);
                    if (film == null || actorFilmRepo.GetRelationExistence(actorId, filmId))
                    {
                        j--;
                        continue;
                    }
                    actorFilmRepo.Insert(actorFilm);
                }
            }
        }

        public static void GenerateFilmActorRelations(int startIndex, int endIndex, SqliteConnection connection)
        {
            var rand = new Random();
            var actorFilmRepo = new ActorFilmRepository(connection);
            var filmRepo = new FilmRepository(connection);
            var actorRepo = new ActorRepository(connection);
            for (int i = startIndex; i < endIndex + 1; i++)
            {
                var actorsCount = rand.Next(1, actorRepo.GetCount() + 1);
                var actorFilm = new ActorFilm();

                var filmId = actorFilm.filmId;
                var film = filmRepo.GetById(i);
                if (film == null)
                {
                    continue;
                }
                actorFilm.filmId = i;

                for (int j = 0; j < actorsCount; j++)
                {
                    var actorId = rand.Next(1, actorRepo.GetMaxId() + 1);
                    var actor = actorRepo.GetById(actorId);
                    if (actor == null || actorFilmRepo.GetRelationExistence(actorId, filmId))
                    {
                        j--;
                        continue;
                    }
                    actorFilm.actorId = actorId;
                    actorFilmRepo.Insert(actorFilm);
                }
            }
        }

        // private static int GetUniqueUserFilmIds(SqliteConnection connection, int usersCount, int filmsCount)
        // {
        //     var rand = new Random();
        //     var userFilmIds = new int[2];
        //     var filmId = rand.Next(1, filmsCount + 1);
        //     var userId = rand.Next(1, usersCount + 1);
        //     var reviewRepo = new ReviewRepository(connection);

        //     while (!reviewRepo.GetRelationExistence(userId, filmId))
        //     {
        //         userId = rand.Next(1, usersCount + 1);
        //         filmId = rand.Next(1, filmsCount + 1);
        //     }
        //     if ()
        //     {
        //         return
        //     }
        // }

        public static TimeSpan GenerateDuration(TimeSpan minDuration, TimeSpan maxDuration)
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

        public static List<User> GenerateUsers(int usersQuantity, DateTime startDate, DateTime endDate)
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

        public static List<Film> GenerateFilms(int filmsQuantity, int startYear,
            int endYear, TimeSpan minDuration, TimeSpan maxDuration)
        {
            var films = new List<Film>();
            var rand = new Random();
            var filmsFilePath = "../data/generator/films.csv";
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

        public static List<Actor> GenerateActors(int actorsQuantity, int minAge, int maxAge)
        {
            var actors = new List<Actor>();
            var rand = new Random();
            var actorsFilePath = "../data/generator/actors.csv";
            var actorsLines = File.ReadAllLines(actorsFilePath);
            var rolePlans = new string[] { "leading", "supporting", "extra" };

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

        public static List<Review> GenerateReviews(int reviewsQuantity,
            DateTime startDate, DateTime endDate, SqliteConnection connection)
        {
            var reviews = new List<Review>();
            var rand = new Random();
            var userRepo = new UserRepository(connection);
            var filmRepo = new FilmRepository(connection);
            var maxUsers = userRepo.GetMaxId();
            var maxFilms = filmRepo.GetMaxId();

            var headersFilePath = "../data/generator/review_headers.txt";
            var overviewsFilePath = "../data/generator/review_overviews.txt";
            var headers = File.ReadAllLines(headersFilePath);
            var overviews = File.ReadAllLines(overviewsFilePath);

            for (int i = 0; i < reviewsQuantity; i++)
            {
                var header = headers[i % headers.Length];
                var overview = overviews[i % overviews.Length];
                var rating = rand.Next(1, 11);
                var lastEdited = GenerateDate(startDate, endDate);

                User user = null;
                Film film = null;
                var filmId = 0;
                var userId = 0;
                while (user == null || film == null)
                {
                    filmId = rand.Next(1, maxFilms + 1);
                    film = filmRepo.GetById(filmId);
                    userId = rand.Next(1, maxUsers + 1);
                    user = userRepo.GetById(userId);
                }

                var review = new Review(header, overview, rating, lastEdited, filmId, userId);
                reviews.Add(review);
            }
            return reviews;
        }

        public static void GeneratePlot(double[] ys)
        {
            var plt = new ScottPlot.Plot(600, 400);

            int pointCount = 10;
            double[] xs = DataGen.Consecutive(pointCount);

            plt.PlotBar(xs, ys, horizontal: true);
            plt.Axis(x1: 0);
            plt.Grid(enableHorizontal: false, lineStyle: LineStyle.Dot);

            string[] labels = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
            plt.YTicks(xs, labels);
            plt.SaveFig("./../ratings.png");
        }
    }
}