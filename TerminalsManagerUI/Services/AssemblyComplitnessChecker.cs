using System.Collections.Generic;
using System.Linq;
using TerminalsManagerUI.ViewModels;

namespace TerminalsManagerUI.Services
{
    public class AssemblyComplitnessChecker
    {
        private readonly IEnumerable<ViewModelAssembly> _vmAssemblies;

        public AssemblyComplitnessChecker(IEnumerable<ViewModelAssembly> vmAssemblies)
        {
            _vmAssemblies = vmAssemblies;
        }

        public IEnumerable<ViewModelAssembly> GetErrors()
        {
            var result = new List<ViewModelAssembly>();
            foreach (var vmAssembly in _vmAssemblies)
            {
                var numbsOfCable = vmAssembly.GetAssembly.Device.NumbersOfCable;
                if (numbsOfCable == vmAssembly.VMCables.Count) continue;
                result.Add(vmAssembly);
            }

            return result;
        }
    }
}