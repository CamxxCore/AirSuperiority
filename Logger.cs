using System;
using System.IO;

namespace AirSuperiority
{
    /// <summary>
    /// Static logger class that allows direct logging of anything to a text file
    /// </summary>
    public static class Logger
    {
        public static void Log(object message)
        {
            try
            {
                File.AppendAllText("GTAV_CustomNeons.log", DateTime.Now + " : " + message + Environment.NewLine);
            }

            catch (UnauthorizedAccessException)
            {             
            }
        }
    }
}
