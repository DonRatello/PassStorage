using System;

namespace PassStorage.Base.DAL
{
    public class Password
    {
        public int id;
        public string title;
        public string login;
        public string password;
        public DateTime creationDate;

        public string GetDays()
        {
            var diff = DateTime.Now.Subtract(creationDate);
            return Math.Round(diff.TotalDays).ToString();
        }

        public bool IsOvertime()
        {
            var diff = DateTime.Now.Subtract(creationDate);
            int dayLimit = Int32.Parse(Config.Instance.DaysWarning);

            return (Math.Round(diff.TotalDays) > dayLimit);
        }

        public string GetWarning()
        {
            int dayLimit = Int32.Parse(Config.Instance.DaysWarning);

            if (IsOvertime()) return $"Password was set more than {dayLimit} days ago!";

            return null;
        }
    }
}
