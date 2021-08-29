using TerminalsManagerUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalsManagerUI.Services.DataRepository
{
    public interface ICableRepository : IRepository<Cable>
    {
        IEnumerable<Cable> GetAllCables();
    }
}
