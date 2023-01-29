using System.Xml.Serialization;
using System.Collections.Generic;

namespace MyConsoleProject
{
    [XmlRoot("root")]
    public class Reviews
    {
        [XmlElement("review")]
        public List<Review> reviews;
    }
}