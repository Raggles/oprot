using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace oprot.plot.core
{
    /// <summary>
    /// Parses a custom curve file.
    /// </summary>
    public class CustomCurveParser
    {
        /// <summary>
        /// Parses a custom curve file
        /// </summary>
        /// <param name="filename">The file to parse</param>
        /// <returns>A dictionary mapping the curve names to an array of points</returns>
        public static Dictionary<string, Point[]> ParseFile(string filename)
        {
            var file = File.OpenText(filename);
            var line = file.ReadLine();
            Dictionary<string, Point[]> result = new Dictionary<string, Point[]>();

            string name = "";
            List<string> values = new List<string>();
            bool first = true;

            while (!file.EndOfStream)
            {
                if (line.StartsWith("#"))
                {
                    if (!first)
                    {
                        result.Add(name, GetArray(values));
                        values.Clear();
                    }
                    name = line.Substring(1, line.Length - 1);
                }
                else
                {
                    if (!string.IsNullOrEmpty(line))
                        values.Add(line);
                }
                first = false;
                line = file.ReadLine();
            }
            result.Add(name, GetArray(values));
            return result;
        }


        private static Point[] GetArray(List<string> list)
        {
            var res = new Point[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                res[i] = Point.Parse(list[i]);
            }
            return res;
        }
    }
}
