using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;

namespace AirSuperiority.Data
{
    public static class INIHelper
    {
        public static string IniPath = string.Format("scripts\\{0}.ini", Assembly.GetExecutingAssembly().GetName().Name);
        public static IniFile IniFile = new IniFile(IniPath);

        /// <summary>
        /// Write a string value to the config file at the specified section and key
        /// </summary>
        /// <param name="section">The section in the config file</param>
        /// <param name="key">The key of the config string</param>
        /// <param name="value">The value of the config string</param>
        private static void WriteValue(string section, string key, string value)
        {
            IniFile.IniWriteValue(section, key, value);
        }

        /// <summary>
        /// Read string from config file with the specified section and key
        /// </summary>
        /// <param name="section">The section in the config file</param>
        /// <param name="key">The key of the config string</param>
        /// <returns></returns>
        private static string ReadValue(string section, string key)
        {
            return IniFile.IniReadValue(section, key);
        }

        /// <summary>
        /// Gets a config setting
        /// </summary>
        /// <param name="section">The section of the config file</param>
        /// <param name="key">The config setting</param>
        /// <returns></returns>
        public static T GetConfigSetting<T>(string section, string key)
        {
            if (!File.Exists(IniPath))
            {
                Create();
            }

            Type type = typeof(T);

            if (type == typeof(bool))
            {
                object setting = Convert.ToBoolean(ReadValue(section, key));
                return (T)setting;
            }
            else if (type == typeof(int))
            {
                object setting = Convert.ToInt32(ReadValue(section, key));
                return (T)setting;
            }
            else if (type == typeof(uint))
            {
                object setting = Convert.ToUInt32(ReadValue(section, key));
                return (T)setting;
            }
            else if (type == typeof(float))
            {
                object setting = (float)Convert.ToDouble(ReadValue(section, key));
                return (T)setting;
            }
            else if (type == typeof(double))
            {
                object setting = Convert.ToDouble(ReadValue(section, key));
                return (T)setting;
            }
            else if (type == typeof(string))
            {
                object setting = ReadValue(section, key);
                return (T)setting;
            }

            else if (type == typeof(Keys))
            {
                object setting = Enum.Parse(typeof(Keys), ReadValue(section, key), true);
                return (T)setting;
            }

            else if (type == typeof(List<Color>))
            {
                var list = new List<System.Drawing.Color>();
                string[] setting = ReadValue(section, key).Split(',');
                Array.ForEach(setting, item => list.Add(Color.FromName(item.Trim())));
                return (T)(object)list;
            }
            else
                throw new ArgumentException("Not a known type.");
        }


        public static void Create()
        {
            try
            {
                if (File.Exists(IniPath)) File.Delete(IniPath);
                Logger.Log("Creating configuration file...");
                IList<string> list = Utils.ReadEmbeddedResource(Properties.Resources.AirSuperiority);
                Utils.WriteListToFile(list, IniPath);
            }

            catch
            {
                Logger.Log("Error creating INI file.");
            }
        }
    }
}
