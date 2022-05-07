using Newtonsoft.Json;
using System;
using System.IO;

namespace MicroAnalys
{
    public class Settings
    {
        public int Exp;
        public int Scale;
        public int GraphSize;
        public ComputerVision.SearchParams SearchParams;
        public string LastFileName;
        private const string SETTINGS_FILE_NAME = "settings.ini";
        public static Settings LoadSettings()
        {
            if (File.Exists(SETTINGS_FILE_NAME))
            {
                try
                {
                    return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SETTINGS_FILE_NAME));
                }
                catch { }
            }
            return CreateNewSettings();
        }

        private static Settings CreateNewSettings()
        {
            var settings = new Settings();
            settings.Scale = 10;
            settings.Exp = -9;
            settings.GraphSize = 10;
            settings.SearchParams = ComputerVision.SearchParams.GenerateParams();
            settings.LastFileName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            return settings;
        }

        public void SaveSettings()
        {
            File.WriteAllText(SETTINGS_FILE_NAME, JsonConvert.SerializeObject(this)); 
        }
    }
}
