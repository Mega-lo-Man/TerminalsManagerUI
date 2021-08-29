using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalsManagerUI.Models
{
    /// <summary>
    /// Perimeter assembly of security and fire alarm systems.
    /// </summary>
    public interface IAssembly : ICloneable
    {
        /// <summary>
        /// Perimetric detector of the security and fire alarm system. 
        /// </summary>
        PerimeterDevice Device { get; set; }

        /// <summary>
        /// Perimeter cable for a fire alarm detector. 
        /// </summary>
        Cable PerimeterCable { get; set; }       
    }
}
