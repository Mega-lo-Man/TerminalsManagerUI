using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalsManagerUI.Services.DataRepository
{
    internal class LocalSettingsUnpackaged : ILocalSettings
    {
        private readonly string _storageFolder;
        private readonly string _fullPath;
        private const string _storageFile = "terminaluimanagersettingsstorage.txt";
            
        public LocalSettingsUnpackaged()
        {
            _storageFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _fullPath = Path.Combine(_storageFolder, _storageFile);

        }

        public object GetSetting(string key)
        {
            var sumString = File.ReadLines(_fullPath);

            var strArray = sumString.ElementAt(0).Split(" ");
            var intList = new List<int>();
            foreach (var str in strArray)
            {
                if (int.TryParse(str, out var value))
                    intList.Add(value);
            }
            return intList;
        }

        public void SetSetting(string key, string value)
        {
            using FileStream fs = File.Open(_fullPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            Byte[] info = new UTF8Encoding(true).GetBytes(value);
            // Add some information to the file.
            fs.Write(info, 0, info.Length);
        }
    }
}
