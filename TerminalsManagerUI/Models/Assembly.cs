using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalsManagerUI.Models
{
    public class Assembly
    {
        #region Constructors

        [JsonConstructor]
        public Assembly()
        {
        }

        public Assembly(PerimeterDevice device)
        {
            Device = device;
        }

        public Assembly(PerimeterDevice device, Cable perimeterCable)
        {
            Device = device;
            PerimeterCables.Append(perimeterCable);
        }

        public Assembly(PerimeterDevice device, List<Cable> perimeterCable)
        {
            Device = device;
            PerimeterCables = perimeterCable;
        }

        #endregion

        public PerimeterDevice Device { get; set; }
        public List<Cable> PerimeterCables { get; set; }

        public object Clone()
        {
            var newDevice = (PerimeterDevice)Device.Clone();
            List<Cable> newPerimeterCables = null;
            if (PerimeterCables != null)
            {
                foreach (var cable in PerimeterCables)
                {
                    newPerimeterCables.Append((Cable)cable.Clone());
                }
            }

            var newAssembly = new Assembly(newDevice, newPerimeterCables);

            return newAssembly;
        }
    }
}
