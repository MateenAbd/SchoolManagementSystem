//using Microsoft.Extensions.Logging;
using NLog;
using SMS.Core.Logger.Interfaces;
using SMS.Core.Logger.Interfaces;
using System;

namespace SMS.Core.Logger.Services
{
    public class LogService : ILog
    {
        private static readonly ILogger Logger = LogManager.GetLogger("FileAndDBLogger");

        public void Debug(string message)
        {
            Logger.Debug(message);
        }

        public void Error(Exception ex, string message)
        {
            Logger.Error(ex, message);
        }

        public void Information(string message)
        {
            Logger.Info(message);
        }

        public void Warning(string message)
        {
            Logger.Warn(message);
        }
    }
}