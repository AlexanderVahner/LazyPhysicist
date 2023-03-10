using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyPhysicist.Common
{
    public enum LogMessageType { Info, Debug, Error, Warning, Default }
    public static class Logger
    {
        public static void Write(object sender, string message, LogMessageType type = LogMessageType.Default) => Logged?.Invoke(sender, message, type);

        public delegate void LoggedEventHandler(object sender, string message, LogMessageType type);
        public static event LoggedEventHandler Logged;
    }
}
