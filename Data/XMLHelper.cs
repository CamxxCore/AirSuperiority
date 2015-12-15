using AirSuperiority.Types;
using System.Collections.Generic;
using System.Xml;
using System;

namespace AirSuperiority.Data
{
    public static class XMLHelper
    {
        public static List<Dictionary<string, string>> ReadValues(string fileName, string dataType, params string[] attributes)
        {
            var data = new List<Dictionary<string, string>>();

            var reader = XmlReader.Create(fileName);

            while (reader.Read())
            {
                if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == dataType))
                {
                    if (reader.HasAttributes)
                    {
                        var dict = new Dictionary<string, string>();

                        for (int i = 0; i < attributes.Length; i++)
                        {
                            dict.Add(attributes[i], reader.GetAttribute(attributes[i]));
                        }

                        data.Add(dict);
                    }
                }
            }

            reader.Dispose();

            return data;
        }

        public static T[] ReadValues<T>(string fileName, string dataType, params string[] attributes)
        {
            var type = typeof(T);

            var xmlData = ReadValues(fileName, dataType, attributes);

            if (type == typeof(TeamInfo))
            {
                var data = new TeamInfo[xmlData.Count];

                for (int i = 0; i < data.Length; i++)
                {
                    var info = new TeamInfo();
                    info.FriendlyName = xmlData[i]["name"];
                    info.ImageAsset = @"scripts\AirSuperiority\" + xmlData[i]["imageAsset"];
                    info.AltImageAsset = @"scripts\AirSuperiority\" + xmlData[i]["altAsset"];
                    data[i] = info;
                }

                return (T[])(object)data;
            }

            else if (type == typeof(HUDAsset))
            {
                var data = new HUDAsset[xmlData.Count];

                for (int i = 0; i < data.Length; i++)
                {
                    var info = new HUDAsset();
                    info.ActiveAsset = @"scripts\AirSuperiority\" + xmlData[i]["activeIcon"];
                    info.InactiveAsset = @"scripts\AirSuperiority\" + xmlData[i]["inactiveIcon"];
                    data[i] = info;
                }
                return (T[])(object)data;
            }

            else throw new ArgumentException("Unknown type.");
        }

    }
}
