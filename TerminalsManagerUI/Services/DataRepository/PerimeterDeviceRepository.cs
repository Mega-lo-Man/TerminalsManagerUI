using TerminalsManagerUI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalsManagerUI.Services.DataRepository
{
    public class PerimeterDeviceRepository : Repository<PerimeterDevice>, IPerimeterDeviceRepository
    {
        public PerimeterDeviceRepository(DbContext context) : base(context)
        {
        }

        public IEnumerable<PerimeterDevice> GetAllPerimeterDevices()
        {
            return ModelDbContext.PerimeterDevices.ToList();
        }

        public ModelDbContext ModelDbContext
        {
            get => Context as ModelDbContext;
        }
    }
}
