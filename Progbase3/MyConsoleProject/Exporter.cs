using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MyConsoleProject
{
    public static class Exporter
    {
        public static Reviews DeserializeReviews(string filePath)
        {
            var reader = new StreamReader(filePath);
            var ser = new XmlSerializer(typeof(Reviews));
            var reviews = (Reviews)ser.Deserialize(reader);
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
    }
}