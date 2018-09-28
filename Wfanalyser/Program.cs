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
                    bool havePunctSeparator = false;
                    foreach (var line in File.ReadLines(filename))
                    {
                        var words = line.Split();

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
                                if (Char.IsUpper(newWord.First()) && !havePunctSeparator && !newWord.All(Char.IsUpper))
                                {
                                    properNouns.Add(newWord);
                                }
                            }


                            if (word.Any())
                            {
                                var last = word.Last();
                                if (char.IsPunctuation(last) && last != ',')
                                {
                                    havePunctSeparator = true;
                                }
                                else
                                {
                                    havePunctSeparator = false;
                                }
                            }
                        }
                        if (words.Length == 1)
                        {
                            havePunctSeparator = true;
                        }
                    }
                }

                using (var str = new StreamWriter("results.txt"))
                {
                    str.WriteLine($"The following words are probably proper nouns:");
                    int wc = 1;
                    foreach (var properNoun in properNouns)
                    {
                        str.Write($"{properNoun} ");
                        if (wc % 8 == 0)
                        {
                            str.WriteLine();
                        }
                        wc++;
                    }
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
