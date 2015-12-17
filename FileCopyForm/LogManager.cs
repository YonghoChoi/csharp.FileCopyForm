using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCopyForm
{
    public enum eLOG_LEVEL
    {
        TRACE,
        DEBUG,
        INFO,
        WARN,
        ERROR,
        FATAL
    }

    static public class LogManager
    {
        private static object writeLock = new object();
        private static FileQueue fileQueue = new FileQueue();
        public static void WriteLog(eLOG_LEVEL logLevel, string format)
        {
            Write(logLevel, format);
        }

        public static void WriteLog(eLOG_LEVEL logLevel, string format, params object[] arg)
        {
            Write(logLevel, string.Format(format, arg));
        }

        private static void Write(eLOG_LEVEL logLevel, string msg)
        {
            lock (writeLock)
            {
                string filePath = string.Format("{0}\\log\\log.txt", AppDomain.CurrentDomain.BaseDirectory);
                fileQueue.CheckTargetFileAndCreate(filePath);
                using (StreamWriter file = new StreamWriter(filePath, true))
                {
                    file.WriteLine(string.Format("{0} : [{1}] {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), logLevel.ToString(), msg));
                }
            }
        }
    }
}
