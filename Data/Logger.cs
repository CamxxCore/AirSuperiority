using System;
using System.IO;

namespace AirSuperiority.Data
{
    /// <summary>
    /// Static logger class that allows direct logging of anything to a text file
    /// </summary>
    public static class Logger
    {
        public static void Log(object message)
        {
            File.AppendAllText("GTAV_CustomNeons.log", DateTime.Now + " : " + message + Environment.NewLine);
        }
    }
}
