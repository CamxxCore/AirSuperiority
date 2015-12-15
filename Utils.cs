using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public static class Utils
{
    /// <summary>
    /// Extension for getting a random item from a list
    /// </summary>
    /// <typeparam name="T">Type of list</typeparam>
    /// <param name="items">The list</param>
    /// <returns></returns>
    public static T GetRandomItem<T>(this IEnumerable<T> items)
    {
        if (items.Count() < 1) return default(T);
        var random = new Random(Guid.NewGuid().GetHashCode());
        return (T)(object)items.ToArray()[random.Next(0, items.Count())];
    }

    /// <summary>
    /// Extension for getting a random loot item
    /// </summary>
    /// <typeparam name="T">Type of list</typeparam>
    /// <param name="list">The list</param>
    /// <returns></returns>
    public static T GetRandomItem<T>(this Enum items)
    {
        var types = Enum.GetValues(typeof(T));
        return types.Cast<T>().GetRandomItem();
    }

    /// <summary>
    /// Concatenates an array of strings with each member on a new line.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string[] GetLines(this string s)
    {
        return s.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
    }

    /// <summary>
    /// Populates a list of strings from an embedded string resource.
    /// </summary>
    /// <param name="resource">The string resource (Properties.Resources.ProjectName...)</param>
    /// <returns></returns>
    public static IList<string> ReadEmbeddedResource(string resource)
    {
        string[] text = resource.GetLines();
        return new List<string>(text);
    }

    /// <summary>
    /// Writes a list of strings to a file at the specified path.
    /// </summary>
    /// <param name="list">The list to write</param>
    /// <param name="filepath">The specified path</param>
    public static void WriteListToFile(IList<string> list, string filepath)
    {
        if (File.Exists(filepath)) File.Delete(filepath);
        using (StreamWriter stream = new StreamWriter(filepath))
        {
            foreach (string line in list)
            {
                stream.WriteLine(line);
            }
        }
    }
}
