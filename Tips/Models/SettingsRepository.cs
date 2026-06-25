using System.Linq;

namespace Tipset.Models
{
    public class SettingsRepository
    {
        private readonly Tips_Entities db;
        private readonly int           _year;

        public SettingsRepository(Tips_Entities db, int year = 2026)
        {
            this.db = db;
            _year = year;
        }

        public string Get(string key, string defaultValue = null)
        {
            var s = db.AppSettings.Find(_year, key);
            return s != null ? s.Value : defaultValue;
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            var val = Get(key);
            bool result;
            return bool.TryParse(val, out result) ? result : defaultValue;
        }

        public void Set(string key, string value)
        {
            var s = db.AppSettings.Find(_year, key);
            if (s == null)
                db.AppSettings.Add(new AppSetting { Year = _year, Key = key, Value = value });
            else
                s.Value = value;
            db.SaveChanges();
        }
    }
}