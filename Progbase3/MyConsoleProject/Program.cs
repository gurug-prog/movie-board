using System;
using Microsoft.Data.Sqlite;
using System.IO;
using Terminal.Gui;

namespace MyConsoleProject
{
    public class Program
    {
        static string exportDirectory;
        static string importFilePath;
        static SqliteConnection connection;

        public static void Main(string[] args)
        {
            var databaseFilePath = "../../data/database.db";
            var fileInfo = new FileInfo(databaseFilePath);
            if (!fileInfo.Exists)
            {
                Console.Error.WriteLine("Cannot create connection to DB: database file not found.");
                return;
            }
            connection = new SqliteConnection($"Data Source={databaseFilePath}");
            // this conditional needed when user don't wanna generate entities
            if (args.Length == 0)
            {

                var userRepo = new ActorRepository(connection);

                Application.Init();
                var top = Application.Top;
                var win = new ActorWindow();

                var menu = new MenuBar(new MenuBarItem[] {
                    new MenuBarItem ("_File", new MenuItem [] {
                        new MenuItem ("_Export...","", OnExport),
                        new MenuItem ("_Import...","", OnImport),
                        new MenuItem ("_Exit", "", OnExit)
                    }),
                    new MenuBarItem ("_Help", new MenuItem [] {
                        new MenuItem ("_GetPlor", "", OnGetPlot),
                        new MenuItem ("_About", "", OnAbout)
                    }),
                });

                win.SetRepository(userRepo);
                top.Add(win, menu);

                Application.Run();
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

        static void OnExit()
        {
            Application.RequestStop();
        }

        static void OnAbout()
        {
            Dialog dialog = new Dialog();
            Label authorName = new Label(2, 4, "Author: Baturkin Ivan KP-03");
            dialog.Add(authorName);
            Button backBtn = new Button("Back");
            dialog.AddButton(backBtn);
            backBtn.Clicked += OnExit;
            Application.Run(dialog);
        }

        static void OnExport()
        {
            OpenDialog dialog = new OpenDialog("Export", "Choose directory to export...");
            NStack.ustring filePath = null;
            dialog.CanChooseDirectories = true;
            dialog.CanChooseFiles = false;
            Application.Run(dialog);

            if (!dialog.Canceled)
            {
                filePath = dialog.FilePath;
            }
            else
            {
                return;
            }

            exportDirectory = filePath.ToString();
            Exporter.ExportReviews(28, exportDirectory, connection);
        }

        static void OnImport()
        {
            OpenDialog dialog = new OpenDialog("Import", "Choose XML file...");
            NStack.ustring filePath = null;
            var reviewRepo = new ReviewRepository(connection);
            Application.Run(dialog);

            if (!dialog.Canceled)
            {
                filePath = dialog.FilePath;
            }
            else
            {
                return;
            }

            importFilePath = filePath.ToString();
            Exporter.ImportReviews(importFilePath, connection);
        }

        static void OnGetPlot()
        {
            var reviewRepo = new ReviewRepository(connection);
            var ys = reviewRepo.GetPlotData();
            Generator.GeneratePlot(ys);
        }
    }
}