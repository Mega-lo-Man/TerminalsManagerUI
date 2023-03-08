using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalsManagerUI.Services
{
    public interface ILocalSettingsSaver
    {
        public void SetPerimeterDevicesIdSetting(IList<int> ids);
        public IEnumerable<int> GetPerimeterDevicesIdSetting();
    }
}
