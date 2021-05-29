using System;
using Microsoft.Data.Sqlite;
// using System.Collections.Generic;
using System.IO;
using Terminal.Gui;

namespace MyConsoleProject
{
    class Program
    {
        static int[] ValidateYears(string[] args)
        {
            var yearsInterval = args[2];
            var yearsBoundsValues = yearsInterval.Split('-');
            if (yearsBoundsValues.Length != 2)
            {
                Console.Error.WriteLine("You have entered wrong years interval.");
                return null;
            }

            int startYear, endYear;
            var startYearIsNotCorrect = !int.TryParse(yearsBoundsValues[0], out startYear) || startYear < 0;
            var endYearIsNotCorrect = !int.TryParse(yearsBoundsValues[1], out endYear) || endYear < 0;
            if (startYearIsNotCorrect || endYearIsNotCorrect || startYear > endYear)
            {
                Console.Error.WriteLine("Years must be non-negative integer numbers,");
                Console.Error.WriteLine("such that startYear <= endYear.");
                return null;
            }

            return new int[] { startYear, endYear };
        }

        static DateTime[] ValidateDates(string[] args)
        {
            var dateIntervalString = args[2];
            var dateBoundsValues = dateIntervalString.Split('-');
            if (dateBoundsValues.Length != 2)
            {
                Console.Error.WriteLine("You have entered wrong date interval.");
                return null;
            }

            DateTime startDate, endDate;
            var startDateIsNotCorrect = !DateTime.TryParse(dateBoundsValues[0], out startDate);
            var endDateIsNotCorrect = !DateTime.TryParse(dateBoundsValues[1], out endDate);
            if (startDateIsNotCorrect || endDateIsNotCorrect || startDate > endDate)
            {
                Console.Error.WriteLine("Dates that you have entered are not correct.");
                return null;
            }

            return new DateTime[] { startDate, endDate };
        }

        static TimeSpan[] ValidateDurations(string[] args)
        {
            var durationInterval = args[3];
            var durationBoundsValues = durationInterval.Split('-');
            if (durationBoundsValues.Length != 2)
            {
                Console.Error.WriteLine("You have entered wrong durations interval.");
                return null;
            }

            TimeSpan minDuration, maxDuration;
            var minDurationIsNotCorrect = !TimeSpan.TryParse(durationBoundsValues[0], out minDuration);
            var maxDurationIsNotCorrect = !TimeSpan.TryParse(durationBoundsValues[1], out maxDuration);
            if (minDurationIsNotCorrect || maxDurationIsNotCorrect || minDuration > maxDuration)
            {
                Console.Error.WriteLine("Durations that you have entered are not correct.");
                return null;
            }

            return new TimeSpan[] { minDuration, maxDuration };
        }

        static int[] ValidateAges(string[] args)
        {
            var agesInterval = args[2];
            var agesBoundsValues = agesInterval.Split('-');
            if (agesBoundsValues.Length != 2)
            {
                Console.Error.WriteLine("You have entered wrong ages interval.");
                return null;
            }

            int minAge, maxAge;
            var minAgeIsNotCorrect = !int.TryParse(agesBoundsValues[0], out minAge) || minAge < 0;
            var maxAgeIsNotCorrect = !int.TryParse(agesBoundsValues[1], out maxAge) || maxAge < 0;
            if (minAgeIsNotCorrect || maxAgeIsNotCorrect || minAge > maxAge)
            {
                Console.Error.WriteLine("Ages must be non-negative integer numbers,");
                Console.Error.WriteLine("such that minAge <= maxAge.");
                return null;
            }

            return new int[] { minAge, maxAge };
        }

        static bool GetActorUserChoice(SqliteConnection connection)
        {
            var filmRepo = new FilmRepository(connection);
            if (filmRepo.GetCount() == 0)
            {
                Console.WriteLine("Database has no films yet. You may generate actors without film relations.");
                Console.WriteLine("Press 'y' key to continue.");
                Console.WriteLine("Press any other key to exit the program.");
                var keyInfo = new ConsoleKeyInfo();
                keyInfo = Console.ReadKey();
                if (keyInfo.Key != ConsoleKey.Y)
                {
                    return false;
                }
            }
            return true;
        }

        static bool GetFilmUserChoice(SqliteConnection connection)
        {
            var actorRepo = new ActorRepository(connection);
            if (actorRepo.GetCount() == 0)
            {
                Console.WriteLine("Database has no actors yet. You may generate films without actor relations.");
                Console.WriteLine("Press 'y' key to continue.");
                Console.WriteLine("Press any other key to exit the program.");
                var keyInfo = new ConsoleKeyInfo();
                keyInfo = Console.ReadKey();
                if (keyInfo.Key != ConsoleKey.Y)
                {
                    return false;
                }
            }
            return true;
        }

        static void ProcessWriteUsers(string[] args, int entitiesQuantity, SqliteConnection connection)
        {
            if (args.Length != 3)
            {
                Console.Error.WriteLine("You have entered more than needed arguments.");
                return;
            }

            var dates = ValidateDates(args);
            if (dates == null)
            {
                return;
            }
            var startDate = dates[0];
            var endDate = dates[1];


            var users = Generator.GenerateUsers(entitiesQuantity, startDate, endDate);
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
                Console.Error.WriteLine("You have entered more than needed arguments.");
                return;
            }

            var userChoice = GetFilmUserChoice(connection);
            if (!userChoice)
            {
                return;
            }

            var years = ValidateYears(args);
            if (years == null)
            {
                return;
            }
            var startYear = years[0];
            var endYear = years[1];

            var durations = ValidateDurations(args);
            if (durations == null)
            {
                return;
            }
            var minDuration = durations[0];
            var maxDuration = durations[1];

            var filmRepo = new FilmRepository(connection);
            var actorRepo = new ActorRepository(connection);
            var startIndex = filmRepo.GetMaxId() + 1;
            var films = Generator.GenerateFilms(entitiesQuantity, startYear,
                endYear, minDuration, maxDuration);
            foreach (var film in films)
            {
                filmRepo.Insert(film);
            }

            var endIndex = filmRepo.GetMaxId();
            if (actorRepo.GetCount() != 0)
            {
                Generator.GenerateFilmActorRelations(startIndex, endIndex, connection);
            }
        }

        static void ProcessWriteActors(string[] args, int entitiesQuantity, SqliteConnection connection)
        {
            if (args.Length != 3)
            {
                Console.Error.WriteLine("You have entered more than needed arguments.");
                return;
            }

            var userChoice = GetActorUserChoice(connection);
            if (!userChoice)
            {
                return;
            }

            var ages = ValidateAges(args);
            if (ages == null)
            {
                return;
            }
            var minAge = ages[0];
            var maxAge = ages[1];

            var filmRepo = new FilmRepository(connection);
            var actorRepo = new ActorRepository(connection);
            var startIndex = actorRepo.GetMaxId() + 1;
            var actors = Generator.GenerateActors(entitiesQuantity, minAge, maxAge);
            foreach (var actor in actors)
            {
                actorRepo.Insert(actor);
            }

            var endIndex = actorRepo.GetMaxId();
            if (filmRepo.GetCount() != 0)
            {
                Generator.GenerateActorFilmRelations(startIndex, endIndex, connection);
            }
        }

        static void ProcessWriteReviews(string[] args, int entitiesQuantity, SqliteConnection connection)
        {
            if (args.Length != 3)
            {
                Console.Error.WriteLine("You have entered more than needed arguments.");
                return;
            }

            var dates = ValidateDates(args);
            if (dates == null)
            {
                return;
            }
            var startDate = dates[0];
            var endDate = dates[1];

            var userRepo = new UserRepository(connection);
            var usersCount = userRepo.GetCount();
            if (usersCount == 0)
            {
                Console.Error.WriteLine("Unable to generate reviews: database has no users.");
                Console.WriteLine("You should generate users or input them via UI.");
                return;
            }

            var filmRepo = new FilmRepository(connection);
            var filmsCount = filmRepo.GetCount();
            if (filmsCount == 0)
            {
                Console.Error.WriteLine("Unable to generate reviews: database has no films.");
                Console.WriteLine("You should generate films or input them via UI.");
                return;
            }

            var reviews = Generator.GenerateReviews(entitiesQuantity, startDate, endDate, connection);
            var reviewRepo = new ReviewRepository(connection);

            foreach (var review in reviews)
            {
                reviewRepo.Insert(review);
            }
        }

        static void Main(string[] args)
        {
            // args = new string[] { "film", "12", "2013-2020", "02:23:20-02:25:50" };
            // args = new string[] { "actor", "5", "19-60"/* , ""  */};
            // args = new string[] { "user", "5", "19.12.1992-29.4.2020"/* , ""  */};
            var databaseFilePath = "../../data/database.db";
            var fileInfo = new FileInfo(databaseFilePath);
            if (!fileInfo.Exists)
            {
                Console.Error.WriteLine("Cannot create connection to DB: database file not found.");
                return;
            }
            SqliteConnection connection = new SqliteConnection($"Data Source={databaseFilePath}");
            // this conditional needed when user don't wanna generate entities

            if (args.Length == 0)
            {
                // Application.Init();

                // var window = new Window("Hello");
                // var top = Application.Top;
                // var frame = top.Frame;
                // var window = new Window();
                // window.X = 4;
                // window.Y = 2;
                // window.Width = 10;
                // window.Height = 5;
                // Add(window);

                // Application.Top.Add(window);
                // Application.Run();
                // System.Threading.Thread.Sleep(2000);
                // Application.RequestStop();

                // Application.Init();
                // var top1 = Application.Top;

                // var win1 = new AuthorizationWindow();
                // top1.Add(win1);

                // Application.Run();

                var userRepo = new UserRepository(connection);

                Application.Init();
                var top = Application.Top;

                var win = new MainWindow();
                win.SetRepository(userRepo);
                top.Add(win);

                Application.Run();

                // userRepo = new UserRepository(connection);

                // Application.Init();
                // var top = Application.Top;
                // var win = new Window("Users DB");
                // top.Add(win);

                // var frame = new Rect(2, 8, top.Frame.Width, 20);
                // allUsersListView = new ListView(frame, userRepo.GetAll());
                // allUsersListView.OpenSelectedItem += OnOpenUser;
                // win.Add(allUsersListView);

                // var createNewUserBtn = new Button(2, 4, "Create new user");
                // createNewUserBtn.Clicked += OnCreateButtonClicked;
                // win.Add(createNewUserBtn);

                // Application.Run();


                // var reviewRepo = new ReviewRepository(connection);

                // // serialize reviews
                // var list = reviewRepo.GetByFilmId(121);
                // // var reviews = new Reviews();
                // // reviews.reviews = list;
                // // Exporter.SerializeReviews("./out.xml", reviews);

                // // deserialize reviews
                // // var reviews = Exporter.DeserializeReviews("./out.xml");
                // // foreach (var review in reviews.reviews)
                // // {
                // //     reviewRepo.Insert(review);
                // // }


                // // var actorRepo = new ActorRepository(connection);
                // // Console.WriteLine(actorRepo.GetMaxId());
                // // var userRepo = new UserRepository(connection);
                // // Console.WriteLine(userRepo.GetMaxId());
                return;
            }

            if (args.Length < 3)
            {
                Console.Error.WriteLine("You have entered less than needed command arguments.");
                return;
            }
            var entitiesQuantity = 0;
            var entitiesQuantityIsNotIntegerOrNegativeNumber =
                !int.TryParse(args[1], out entitiesQuantity) || entitiesQuantity < 0;
            if (entitiesQuantityIsNotIntegerOrNegativeNumber)
            {
                Console.Error.WriteLine("Quantity of entities must be positive integer number.");
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
                Console.Error.WriteLine($"Unknown entity: {args[0]}");
                return;
            }
        }
    }
}