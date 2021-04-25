using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

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

        static List<User> GenerateUsers(int usersQuantity, DateTime startDate, DateTime endDate)
        {
            var users = new List<User>();
            var rand = new Random();
            for (int i = 0; i < usersQuantity; i++)
            {
                var login = GenerateLogin();
                var password = GeneratePassword();
                var roles = new string[] { "moderator", "author" };
                var role = roles[rand.Next(0, 2)];
                var signUpDate = GenerateDate(startDate, endDate);

                var user = new User(login, password, role, signUpDate);
                users.Add(user);
            }
            return users;
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
            if (startDateIsNotCorrect || endDateIsNotCorrect)
            {
                Console.WriteLine("Dates that you have entered are not correct.");
                return;
            }

            var users = GenerateUsers(entitiesQuantity, startDate, endDate);
            var userRepo = new UserRepository(connection);
            foreach (var user in users)
            {
                Console.WriteLine(userRepo.Insert(user));
            }
        }

        static void Main(string[] args)
        {
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
            // else if (quantityOfLines > 10000)
            // {
            //     Console.WriteLine("Cannot generate more 10000 entities");
            //     return;
            // }
            var databaseFilePath = "../../data/database.db";
            SqliteConnection connection = new SqliteConnection($"Data Source={databaseFilePath}");

            if (args[0] == "user")
            {
                ProcessWriteUsers(args, entitiesQuantity, connection);
            }
            // else if (args[0] == "film")
            // {
            //     ProcessGenerateFilms(args);
            // }
            // else if (args[0] == "actor")
            // {
            //     ProcessGenerateActors(args);
            // }
            // else if (args[0] == "review")
            // {
            //     ProcessGenerateReviews(args);
            // }
            else
            {
                Console.WriteLine($"Unknown entity: {args[0]}");
                return;
            }
        }
    }
}