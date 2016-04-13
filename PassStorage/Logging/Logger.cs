using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PassStorage.Logging
{
    public class Logger
    {
        private static Logger instance;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
            log4net.ThreadContext.Properties["ASSEMBLY_VERSION"] = Classes.Common.GetVersion();
        }

        private void WriteHeader()
        {
            //TODO implement header
        }

        public void Debug(string message)
        {
            Debug(null, message);
        }

        public void Debug(string functionName, string message)
        {
            
        }

        public void Warning(string message)
        {
            Warning(null, message);
        }

        public void Warning(string functionName, string message)
        {

        }

        public void Error(string message)
        {

        }

        public void Error(Exception e)
        {

        }

        public void Error(string functionName, string message)
        {

        }

        public void Error(string functionName, Exception e)
        {

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