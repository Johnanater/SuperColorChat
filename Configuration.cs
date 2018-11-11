using System.Collections.Generic;
using System.Xml.Serialization;
using Rocket.API;

namespace SuperColorChat
{
    public class Configuration : IRocketPluginConfiguration
    {
        public string DatabaseAddress;
        public int DatabasePort;
        public string DatabaseName;
        public string DatabaseTableName;
        public string DatabaseUsername;
        public string DatabasePassword;

        public bool UseMoney;
        public int Cost;

        [XmlArray(ElementName = "Colors")]
        [XmlArrayItem(ElementName = "Color")]
        public List<Color> Colors;

        public void LoadDefaults()
        {
            DatabaseAddress = "localhost";
            DatabasePort = 3306;
            DatabaseName = "database_name";
            DatabaseTableName = "colors";
            DatabaseUsername = "database_username";
            DatabasePassword = "database_password";

            UseMoney = true;
            Cost = 1000;

            Colors = new List<Color>
            {
                new Color("green", "00ff00"),
                new Color("darkgreen", "32be3c"),
                new Color("red", "ff0000"),
                new Color("yellow", "ffff00"),
                new Color("orange", ""),
                new Color("pink", "ff3399"),
                new Color("bluegreen", "00ff96"),
                new Color("bluegray", "4b5091"),
                new Color("black", "000000"),
                new Color("white", "ffffff"),
                new Color("darkgray", "2d2d2d"),
                new Color("lightgray", "737373")
            };
        }
    }

    public sealed class Color
    {
        [XmlAttribute("Name")]
        public string Name;

        [XmlAttribute("Hex")]
        public string Hex;

        public Color() { }

        public Color(string name, string hex)
        {
            Name = name;
            Hex = hex;
        }
    }
}