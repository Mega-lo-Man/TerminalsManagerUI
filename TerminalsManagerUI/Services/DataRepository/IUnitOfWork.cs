using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalsManagerUI.Services.DataRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ICableRepository Cables { get; }
        IPerimeterDeviceRepository PerimeterDevices { get; }
        int Complete();
    }
}
