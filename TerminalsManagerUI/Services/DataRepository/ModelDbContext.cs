using TerminalsManagerUI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminalsManagerUI.Services.DataRepository
{
    public class ModelDbContext : DbContext
    {
        public virtual DbSet<Cable> Cables { get; set; }
        public virtual DbSet<PerimeterDevice> PerimeterDevices { get; set; }

        static ModelDbContext()
        {
            //Database.SetInitializer<ModelDbContext>(new ContextInitializer());
        }

        public bool IsDbExists 
        {
            get
            {
                try
                {
                    Database.Connection.Open();
                    var result = Database.Exists();
                    Database.Connection.Close();
                    return result;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public ModelDbContext(string connString) : base(connString)//"Server=localhost;Database=acadBlocksDatabase;Trusted_Connection=True;")//("name=DefaultConnection")
        {
            this.Configuration.LazyLoadingEnabled = true;
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PerimeterDevice>().Ignore(p => p.TerminalsList);
            modelBuilder.Entity<BaseComponent>().Ignore(p => p.Designation);
        }
    }
}
