using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Wfanalyser
{


    internal class Program
    {
        public static void IncrementDictEntry<T>(SortedDictionary<T, int> d, T key)
        {
            if (!d.TryGetValue(key, out var v))
            {
                d[key] = 0;
            }
            d[key]++;
        }

        private static void Main(string[] args)
        {
            try
            {
                if (!args.Any())
                {
                    throw new ArgumentException("You must supply one or more filenames");
                }

                var frequencies = new SortedDictionary<string, int>();
                var properNouns = new SortedSet<string>();

                foreach (var filename in args)
                {
                    foreach (var line in File.ReadLines(filename))
                    {
                        var words = line.Split();

                        bool havePunctSeparator = false;
                        foreach (var word in words)
                        {
                            var newWord = "";
                            foreach (var ch in word)
                            {
                                if (char.IsLetter(ch))
                                {
                                    newWord += ch;
                                }
                            }

                            if (newWord.Any())
                            {
                                IncrementDictEntry(frequencies, newWord);
                            }

                            if (Char.IsUpper(newWord.First()) && !havePunctSeparator)
                            {
                                properNouns.Add(newWord);
                            }

                            var last = word.Last();
                            if (char.IsPunctuation(last) && last != ',')
                            {
                                havePunctSeparator = true;
                            }

                        }
                    }
                }

                using (var str = new StreamWriter("results.txt"))
                {
                    str.WriteLine($"{frequencies.Count} different words:");
                    foreach (var entry in frequencies.OrderByDescending(x => x.Value))
                    {
                        str.WriteLine($"{entry.Key} {entry.Value}");
                    }
                }
            }
            catch (Exception ex)
            {
                var codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                var progname = Path.GetFileNameWithoutExtension(codeBase);
                Console.Error.WriteLine(progname + ": Error: " + ex.Message);
            }

        }
    }
}
