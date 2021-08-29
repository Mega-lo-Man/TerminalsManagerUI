using TerminalsManagerUI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalsManagerUI.Services.DataRepository
{
    public class ContextInitializer : CreateDatabaseIfNotExists<ModelDbContext>
    {
        protected override void Seed(ModelDbContext context)
        {
            var dev1 = new PerimeterDevice()
            {
                DeviceName = "Антирис-5.8-20А-01",
                DeviceDescription = "Description1",
                TerminalsList = new List<string> { "+24В", "0В", "ШС+", "ШС-", "ДК+" },
                BlockRef = "VSF1_АНТИРИС",
                //ImagePath = "G:\\Acad\\Lib\\Images\\VSF1_АНТИРИС.png"
            };

            var dev2 = new PerimeterDevice()
            {
                DeviceName = "Виброн-01А",
                DeviceDescription = "Description2",
                TerminalsList = new List<string> { "0В", "24В", "ШС1+", "ШС1-", "ШС2+", "ШС2-", "ДК+" },
                BlockRef = "VSF1_АНТИРИС",
                //ImagePath = "G:\\Acad\\Lib\\Images\\VSF1_АНТИРИС.png"
            };

            var cable1 = new Cable() { Brand = "Brand1", WiresNumber = 4, IsArmoured = false };
            var cable2 = new Cable() { Brand = "Brand2", WiresNumber = 8, IsArmoured = false };
            var cable3 = new Cable() { Brand = "Brand3", WiresNumber = 16, IsArmoured = true };


            context.Cables.Add(cable1);
            context.Cables.Add(cable2);
            context.Cables.Add(cable3);

            context.PerimeterDevices.Add(dev1);
            context.PerimeterDevices.Add(dev2);

            context.SaveChanges();
        }
    }
}
