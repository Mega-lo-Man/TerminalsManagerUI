using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalsManagerUI.Services
{
    public class LocalSettingsSaver : ILocalSettingsSaver
    {
        const string perimeterDevicesIdKey = "CacheIds";
        private ILocalSettings _localSettings;

        public LocalSettingsSaver(ILocalSettings localSettings)
        {
            _localSettings = localSettings;
        }

        public IEnumerable<int> GetPerimeterDevicesIdSetting() => (IEnumerable<int>)_localSettings.GetSetting("");

        public void SetPerimeterDevicesIdSetting(IList<int> ids)
        {
            var idsString = string.Join(" ", ids);
            _localSettings.SetSetting(perimeterDevicesIdKey, idsString);
        }
    }
}
