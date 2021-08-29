using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalsManagerUI.Services.DataRepository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ModelDbContext _context;

        public UnitOfWork(ModelDbContext context)
        {
            _context = context;
            Cables = new CableRepository(_context);
            PerimeterDevices = new PerimeterDeviceRepository(_context);
        }

        public ICableRepository Cables { get; private set; }

        public IPerimeterDeviceRepository PerimeterDevices { get; private set; }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
