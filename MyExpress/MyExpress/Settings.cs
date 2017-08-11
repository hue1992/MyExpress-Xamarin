using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace MyExpress
{
    public static class Settings
    {

        private static ISettings AppSettings =>CrossSettings.Current;

        public static string DefaultUrl
        {
            get => AppSettings.GetValueOrDefault(nameof(DefaultUrl), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(DefaultUrl), value);
        }
    }
}
