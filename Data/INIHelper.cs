using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;

namespace AirSuperiority.Data
{
    public static class INIHelper
    {
        public static readonly string IniPath = string.Format("scripts\\{0}.ini", Assembly.GetExecutingAssembly().GetName().Name);
        public static readonly IniFile IniFile = new IniFile(IniPath);

        static INIHelper()
        {
            if (!File.Exists(IniPath))
                Create();
        }

        /// <summary>
        /// Write a string value to the config file at the specified section and key
        /// </summary>
        /// <param name="section">The section in the config file</param>
        /// <param name="key">The key of the config string</param>
        /// <param name="value">The value of the config string</param>
        public static void WriteValue(string section, string key, string value)
        {
            IniFile.IniWriteValue(section, key, value);
        }

        /// <summary>
        /// Gets a config setting
        /// </summary>
        /// <param name="section">The section of the config file</param>
        /// <param name="key">The config setting</param>
        /// <returns></returns>
        public static T GetConfigSetting<T>(string section, string key, T defaultKey = default(T))
        {
            Type type = typeof(T);
            if (!type.IsValueType)
                throw new ArgumentException("Not a known type.");

            var keyValue = IniFile.IniReadValue(section, key);
            var tConverter = TypeDescriptor.GetConverter(type);

            if (keyValue.Length > 0 && tConverter.CanConvertFrom(typeof(string)))
            {
                return (T)tConverter.ConvertFromString(keyValue);
            }

            else return defaultKey;
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
