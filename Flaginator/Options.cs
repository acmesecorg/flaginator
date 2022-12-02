using CommandLine;

namespace Flaginator
{
    public class FlagOptions
    {
        [Value(0)]
        public IEnumerable<string> FilePaths { get; set; }
    }
}
