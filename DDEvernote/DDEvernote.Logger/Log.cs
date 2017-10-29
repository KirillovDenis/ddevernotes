using NLog;

namespace DDEvernote.Logger
{
    public class Log
    {
        public static readonly NLog.Logger Instance = LogManager.GetCurrentClassLogger();
    }
}
