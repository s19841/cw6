using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cw6.Services
{
    public class FileLogService : ILogService
    {
        private const string LogFilename = "requestsLog.txt";

        public void Log(object message)
        {
            File.AppendAllText(LogFilename, message + Environment.NewLine);
        }
    }
}
