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

            if (File.Exists(Environment.CurrentDirectory + "/settings.json"))
            {
                string settingsString = File.ReadAllText("settings.json");
                Settings settings = JsonConvert.DeserializeObject<Settings>(settingsString);

                ChangeSkin(settings.theme);
            }
        }

        public void ChangeSkin(string newSkin)
        {
            Skin = newSkin;
            Resources.Clear();
            Resources.MergedDictionaries.Clear();
            AddResourceDictionary("Skins/" + Skin + ".xaml");
        }

        private void AddResourceDictionary(string src)
        {
            Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(src, UriKind.Relative) });
        }
    }
}
