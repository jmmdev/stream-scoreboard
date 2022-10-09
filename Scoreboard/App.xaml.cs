using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;

namespace Scoreboard
{
    public enum Skin { Red, Blue }

    public partial class App : Application
    {
        public static string Skin { get; set; } = "Default";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "settings.json"))
            {
                string settingsString = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "settings.json");
                Settings settings = JsonConvert.DeserializeObject<Settings>(settingsString);
                
                if(string.IsNullOrEmpty(settings.theme) || string.IsNullOrWhiteSpace(settings.theme))
                    ChangeSkin("Default");
                else
                    ChangeSkin(settings.theme);

            }
            else
                ChangeSkin("Default");
        }

        public void ChangeSkin(string newSkin)
        {
            Skin = newSkin;
            Resources.Clear();
            Resources.MergedDictionaries.Clear();
            AddResourceDictionary(AppDomain.CurrentDomain.BaseDirectory + "Skins\\" + Skin + ".xaml");
        }

        private void AddResourceDictionary(string src)
        {
            Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(src, UriKind.Absolute) });
        }
    }
}
