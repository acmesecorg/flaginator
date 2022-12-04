using CommandLine;
using System.IO;
using static System.Net.WebRequestMethods;


namespace Flaginator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Parse command line 
            var parsed = Parser.Default.ParseArguments<FlagOptions>(args);
            var options = parsed.Value;

            if (parsed.Tag == ParserResultType.NotParsed)
            {
                ConsoleUtil.WriteError("One or more arguments were invalid");
                return;
            }

            //Validate the file paths
            if (options.FilePaths.Count() == 0)
            {
                ConsoleUtil.WriteError("One or more file paths must be supplied");
                return;
            }

            //Check file exists
            var currentDirectory = Directory.GetCurrentDirectory();
            foreach (var path in options.FilePaths)
            {
                var mergedPath = Path.Combine(currentDirectory, path);
                if (!System.IO.File.Exists(mergedPath))
                {
                    ConsoleUtil.WriteError($"File {mergedPath} not found");
                    return;
                }
            }

            //We want to combine all unique combinations now
            //ie file1 + file 2 + file 3
            //file1 + file3
            //file1
            //file3 + file1
            //But not when using the same file multiple times
            var pathSet = new HashSet<int>();
            var paths = options.FilePaths.ToList();
            var files = new List<List<string>>();

            //Build all unique combinations of files 
            AddPath(pathSet, paths, files, new List<string>(), 0);

            //Display all file paths if debug is enabled
            if (options.Debug)
            {
                Console.Out.WriteLine("Combining the following files:");
                foreach (var fileList in files)
                {
                    Console.Out.WriteLine(string.Join(' ', fileList));
                }
            }

            //Define a hashset to store past word combinations
            var hashset = new HashSet<int>();

            //Loop through each file and progressively output a line if it isnt found
            foreach (var fileList in files)
            {
                MergeFilePaths(hashset, fileList, 0, new List<string>(), options);
            }
        }

        public static void AddPath(HashSet<int> hashset, List<string> paths, List<List<string>> files, List<string> parents, int index)
        {
            foreach (var path in paths)
            {
                var hashPaths = $"{string.Concat(parents)}{path}";
                var hash = hashPaths.GetHashCode();

                if (hashset.Add(hash))
                {
                    var newParents = new List<string>(parents);
                    newParents.Add(path);
                    files.Add(newParents);

                    if (index < paths.Count - 1)
                    {
                        AddPath(hashset, paths, files, newParents, index + 1);
                    }
                }
            }
        }

        public static void MergeFilePaths(HashSet<int> hashset, List<string> paths, int index, List<string> parents, FlagOptions options)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var mergedPath = Path.Combine(currentDirectory, paths[index]);
            var isFinal = (index == paths.Count - 1);

            //First pass, write out lines
            using (var reader = new StreamReader(mergedPath))
            {
                while (!reader.EndOfStream)
                {
                    var word = reader.ReadLine();
                    var flag = false;

                    if (word == null) continue;

                    //Check the word isnt in any opf the parents
                    foreach (var parent in parents)
                    {
                        if (parent == word)
                        {
                            flag = true;
                            break;
                        }
                    }

                    if (flag) continue;

                    //Write out phrase, or iterate
                    if (isFinal)
                    {
                        var final = $"{string.Concat(parents)}{word}";

                        //Check for duplicates
                        var hash = final.GetHashCode();
                        if (hashset.Add(hash))
                        {
                            if (final.Length >= options.MinimumLength && final.Length <= options.MaximumLength)
                            {
                                Console.Out.WriteLine(final);
                            }
                        }
                    }
                    else
                    {
                        parents.Add(word);
                        MergeFilePaths(hashset, paths, index + 1, parents, options);

                        //Remove last word
                        parents.RemoveAt(parents.Count - 1);
                    }
                }
            }
        }
    }
}