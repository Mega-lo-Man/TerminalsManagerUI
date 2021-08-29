using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TerminalsManagerUI.Models;
using TerminalsManagerUI.ViewModels;

namespace TerminalsManagerUI.Services
{
    public class AssemblySaver
    {
        public string TargetFileName { get; set; }

        public bool Save(IEnumerable<ViewModelAssembly> ViewModelAssemblyList)
        {
            var assemblyList = ViewModelAssemblyList.Select(_vmAssy => _vmAssy.GetAssembly).ToList();

            var jsonString = JsonConvert.SerializeObject(assemblyList);
            try
            {
                using var sw = File.CreateText(TargetFileName);
                sw.Write(jsonString);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}