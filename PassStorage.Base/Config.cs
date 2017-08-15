using System.Configuration;

namespace PassStorage.Base
{
    public class Config
    {
        private static Config instance;

        public string EnterHash { get; private set; }
        public string DaysWarning { get; private set; }
        public string Filename { get; private set; }

        private Config() { Read(); }

        public static Config Instance
        {
            get
            {
                if (instance == null) instance = new Config();

                return instance;
            }
        }

        private void Read()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                EnterHash = appSettings["ENTER_HASH"];
                DaysWarning = appSettings["DAYS_WARNING"];
                Filename = appSettings["FILENAME"];
            }
            catch (ConfigurationErrorsException e)
            {

            }
        }
    }
}
