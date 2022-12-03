using CommandLine;

namespace Flaginator
{
    public class FlagOptions
    {
        [Value(0, HelpText = "Paths of files to combine")]
        public IEnumerable<string> FilePaths { get; set; }

        [Option(Default = false, HelpText = "Display additional debug information")]
        public Boolean Debug { get; set; }
    }
}
