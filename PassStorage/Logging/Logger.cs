using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PassStorage.Classes;
using System.Diagnostics;

namespace PassStorage.Logging
{
    public class Logger
    {
        private static Logger instance;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Logger() { }

        public static Logger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Logger();
                    instance.Init();
                    instance.WriteHeader();
                }
                return instance;
            }
        }

        private void Init()
        {
            log4net.ThreadContext.Properties["APPLICATION_NAME"] = "PassStorage";
            log4net.ThreadContext.Properties["ASSEMBLY_VERSION"] = Common.GetVersion();
        }

        private void WriteHeader()
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
            string systemArch = Environment.Is64BitOperatingSystem ? "x64" : "x86";
            string processArch = Environment.Is64BitProcess ? "x64" : "x86";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("--------------------------------------------------------------------------");
            sb.AppendLine("PASS STORAGE");
            sb.AppendLine($"Application:              {versionInfo.ProductName}");
            sb.AppendLine($"Version                   {Common.GetVersion()}");
            sb.AppendLine($"Build                     {Common.GetLinkerTime(Assembly.GetExecutingAssembly()).ToString("yyyyMMddHHmmss")}");
            sb.AppendLine($"Author:                   {versionInfo.CompanyName}");
            sb.AppendLine($"Copyright:                {versionInfo.LegalCopyright}");
            sb.AppendLine($"Operating system:         {Common.GetOperatingSystem()}");
            sb.AppendLine($"System architecture:      {systemArch}");
            sb.AppendLine($"Process architecture:     {processArch}");
            sb.AppendLine();
            sb.AppendLine($"Starting logging at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");

            log.Debug(sb.ToString());
        }

        public void Debug(string message)
        {
            Debug(null, message);
        }

        public void Debug(string functionName, string message)
        {
            if (string.IsNullOrEmpty(functionName))
            {
                log.Debug($"{functionName} :: {message}");
            }
            else
            {
                log.Debug($"{message}");
            }
        }

        public void Warning(string message)
        {
            Warning(null, message);
        }

        public void Warning(string functionName, string message)
        {
            if (string.IsNullOrEmpty(functionName))
            {
                log.Warn($"{functionName} :: {message}");
            }
            else
            {
                log.Debug($"{message}");
            }
        }

        public void Error(string message)
        {
            Error(null, message);
        }

        public void Error(Exception e)
        {
            Error(null, e);
        }

        public void Error(string functionName, string message)
        {
            if (string.IsNullOrEmpty(functionName))
            {
                log.Error($"{functionName} :: {message}");
            }
            else
            {
                log.Error($"{message}");
            }
        }

        public void Error(string functionName, Exception e)
        {
            if (string.IsNullOrEmpty(functionName))
            {
                log.Error($"{functionName} :: {e.Message}");
            }
            else
            {
                log.Error($"{e.Message}");
            }
        }

        public void FunctionStart(string functionName)
        {
            log.Debug($"START {functionName}");
        }

        public void FunctionExit(string functionName)
        {
            log.Debug($"FINISH {functionName}");
        }
    }
}