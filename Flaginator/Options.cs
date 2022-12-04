using CommandLine;

namespace Flaginator
{
    public class FlagOptions
    {
        [Value(0, HelpText = "Paths of files to combine")]
        public IEnumerable<string> FilePaths { get; set; }

        [Option(Default = false, HelpText = "Display additional debug information")]
        public Boolean Debug { get; set; }

        [Option("min", Default = 0, HelpText = "Set minimum line length")]
        public int MinimumLength { get; set; }

        [Option("max", Default = 1024, HelpText = "Set maximum line length")]
        public int MaximumLength { get; set; }

        [Option('r', "rules", HelpText = "Rules to apply to each combination.")]
        public IEnumerable<string> RulePaths { get; set; }
    }
}
