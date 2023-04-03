using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PlansCache
{
    public class Parameters
    {
        public Parameters(string[] args)
        {
            for (int i = 0; i < args.Count(); i++)
            {
                if (args[i] == "-db")
                {
                    DbPath = args[++i];
                    continue;
                }
                if (args[i] == "-all")
                {
                    RecheckAll = true;
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
        public bool DebugMode { get; set; } = false;
        public bool VerboseMode { get; set; } = false;
        public bool RecheckAll { get; set; } = false;
    }
}
