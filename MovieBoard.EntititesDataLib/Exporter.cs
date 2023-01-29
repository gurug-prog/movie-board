using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Data.Sqlite;

namespace MyConsoleProject
{
    public static class Exporter
    {
        public static Reviews DeserializeReviews(string filePath)
        {
            var reader = XmlReader.Create(filePath);
            var ser = new XmlSerializer(typeof(Reviews));
            if (!ser.CanDeserialize(reader))
            {
                Console.Clear();
                Console.Error.WriteLine("Can not deserialize given document.");
                Environment.Exit(0);
            }

            Reviews reviews = null;
            try
            {
                reviews = (Reviews)ser.Deserialize(reader);
            }
            catch (Exception)
            {
                Console.Clear();
                Console.Error.WriteLine("Can not deserialize given document.");
                Environment.Exit(0);
            }
            reader.Close();
            return reviews;
        }

        public static void SerializeReviews(string filePath, Reviews reviews)
        {
            var ser = new XmlSerializer(typeof(Reviews));
            var output = new StreamWriter(filePath);
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineHandling = NewLineHandling.Entitize;
            var writer = XmlWriter.Create(output, settings);
            ser.Serialize(writer, reviews);
        }

        public static void ExportReviews(int filmId, string exportDirectory, SqliteConnection connection)
        {
            var reviewRepo = new ReviewRepository(connection);
            var list = reviewRepo.GetByFilmId(filmId);
            var reviews = new Reviews();
            reviews.reviews = list;
            Exporter.SerializeReviews(exportDirectory + "/out.xml", reviews);
        }

        public static void ImportReviews(string exportFile, SqliteConnection connection)
        {
            var reviewRepo = new ReviewRepository(connection);
            var reviews = Exporter.DeserializeReviews(exportFile);
            foreach (var review in reviews.reviews)
            {
                reviewRepo.Insert(review);
            }
        }
    }
}