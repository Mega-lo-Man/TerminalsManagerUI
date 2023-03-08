using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalsManagerUI.Services
{
    public interface ILocalSettings
    {
        void SetSetting(string key, string value);
        object GetSetting(string key);
    }
}
