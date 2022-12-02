using CommandLine;
using System.IO;


namespace Flaginator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Parse command line 
            var parsed = Parser.Default.ParseArguments<FlagOptions>(args);
            var options = parsed.Value;

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
                if (!File.Exists(mergedPath))
                {
                    ConsoleUtil.WriteError($"File {mergedPath} not found");
                    return;
                }
            }

            //Define a hashset to store past combinations
            var hashset = new HashSet<int>();
            var paths = options.FilePaths.ToList();

            //Loop through each file and progressively output a line if it isnt found
            MergeFilePaths(hashset, paths, 0, "");
        }

        public static void MergeFilePaths(HashSet<int> hashset, List<string> paths, int index, string parent)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var mergedPath = Path.Combine(currentDirectory, paths[index]);

            //First pass, write out line
            using (var reader = new StreamReader(mergedPath))
            {
                while (!reader.EndOfStream)
                {
                    var line = $"{parent}{reader.ReadLine()}";
                    var hash = line.GetHashCode();

                    //If it was added to the hashset, then it should be outputted
                    if (hashset.Add(hash)) Console.Out.WriteLine(line);
                }
            }

            if (index == paths.Count -1) return;

            //Second pass, iterate
            using (var reader = new StreamReader(mergedPath))
            {
                while (!reader.EndOfStream)
                {
                    MergeFilePaths(hashset, paths, index + 1, $"{parent}{reader.ReadLine()}");
                }
            }
        }
    }
}