using System;
using System.Collections.Generic;

namespace PassStorage.Base.DAL
{
    public class Root
    {
        public string date;
        public string version;
        public string user;
        public string computerName;
        public List<Password> data;

        public void FillDefaults()
        {
            computerName = Utils.GetOperatingSystem();
            user = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            date = $"{DateTime.Now.ToShortDateString()}  {DateTime.Now.ToShortTimeString()}";
            version = Utils.GetVersion();
            data = new List<Password>();
        }
    }
}
