using Windows.Storage;

namespace TerminalsManagerUI.Services
{
    public class LocalSettingsPackaged : ILocalSettings
    {
        private readonly ApplicationDataContainer _localSettings;

        public LocalSettingsPackaged()
        {
            _localSettings = ApplicationData.Current.LocalSettings;
        }

        public void SetSetting(string key, string value)
        {
            _localSettings.Values[key] = value;
        }

        public object GetSetting(string key)
        {
            if (_localSettings.Values.ContainsKey(key))
            {
                return _localSettings.Values[key];
            }

            return null;
        }
    }
}

