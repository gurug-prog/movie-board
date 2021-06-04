using System;
using Microsoft.Data.Sqlite;
using System.IO;
using Terminal.Gui;

namespace MyConsoleProject
{
    public class Program
    {
        public static void Main(string[] args)
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

                var userRepo = new ActorRepository(connection);

                Application.Init();
                var top = Application.Top;

                var win = new ActorWindow();
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
                GenerationProcessor.ProcessWriteUsers(args, entitiesQuantity, connection);
            }
            else if (args[0] == "film")
            {
                GenerationProcessor.ProcessWriteFilms(args, entitiesQuantity, connection);
            }
            else if (args[0] == "actor")
            {
                GenerationProcessor.ProcessWriteActors(args, entitiesQuantity, connection);
            }
            else if (args[0] == "review")
            {
                GenerationProcessor.ProcessWriteReviews(args, entitiesQuantity, connection);
            }
            else
            {
                Console.Error.WriteLine($"Unknown entity: {args[0]}");
                return;
            }
        }
    }
}