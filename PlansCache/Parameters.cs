using System;
using System.Linq;

namespace PlansCache
{
    public sealed class Parameters
    {
        public Parameters(string[] args)
        {
            for (int i = 0; i < args.Count(); i++)
            {
                if (args[i] == "-db")
                {
                    DbPath = Environment.ExpandEnvironmentVariables(args[++i]);
                    continue;
                }
                if (args[i] == "-all")
                {
                    RecheckAll = true;
                    continue;
                }
                if (args[i] == "-years")
                {
                    if (int.TryParse(args[++i], out int y) && y < 100)
                    {
                        Years = y;
                    }
                    else
                    {
                        --i;
                    }
                    continue;
                }
                if (args[i] == "-verbose")
                {
                    VerboseMode = true;
                    continue;
                }
                if (args[i] == "-debug")
                {
                    DebugMode = true;
                    VerboseMode = true;
                    continue;
                }
            }
        }
        public string DbPath { get; set; } = "";
        public int Years { get; set; } = 0;
        public bool DebugMode { get; set; } = false;
        public bool VerboseMode { get; set; } = false;
        public bool RecheckAll { get; set; } = false;
    }
}
