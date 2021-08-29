using TerminalsManagerUI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalsManagerUI.Services.DataRepository
{
    public class CableRepository : Repository<Cable>, ICableRepository
    {
        public CableRepository(ModelDbContext context) : base(context)
        {
        }

        public IEnumerable<Cable> GetAllCables()
        {
            return ModelDbContext.Cables.ToList();
        }

        public ModelDbContext ModelDbContext
        {
            get => Context as ModelDbContext;
        }
    }
}
