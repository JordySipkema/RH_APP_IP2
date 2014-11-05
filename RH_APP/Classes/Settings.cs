using Mallaca.Usertypes;

namespace RH_APP.Classes
{
    public class Settings
    {
        private static Settings settings;

        public User CurrentUser { get; set; }
        public string authToken { get; set; }

        private Settings()
        {

        }

        public static Settings GetInstance()
        {
           if(settings == null)
               settings = new Settings();
            return settings;
        }
    }
}
