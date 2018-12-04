using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Linq.Mapping;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oprot.database.core
{
    [Database]
    public class DatabaseContext : DbContext
    {
        
        public DatabaseContext() :
            base(new SQLiteConnection()
            {
                ConnectionString = new SQLiteConnectionStringBuilder() { DataSource =  Properties.Settings.Default.DataPath + "protection.db", ForeignKeys = true }.ConnectionString
            }, true)
        {
        }
        
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<ProtectedPlant>().ToTable("ProtectedPlant");
            modelBuilder.Entity<ProtectedPlant>().HasKey(x => x.ProtectedPlantId);
            modelBuilder.Entity<ProtectedPlant>().Property(x => x.ProtectedPlantId).HasColumnName("rowid").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<ProtectedPlant>().Property(x => x.PlantId).HasColumnName("PlantId");
            modelBuilder.Entity<ProtectedPlant>().Property(x => x.Substation).HasColumnName("Substation");
            modelBuilder.Entity<ProtectedPlant>().Property(x => x.AssetNumber).HasColumnName("AssetNumber");
            modelBuilder.Entity<ProtectedPlant>().HasMany(x => x.ProtectionRelays).WithRequired(x => x.Plant).HasForeignKey(x => x.ProtectedPlantId);


            modelBuilder.Entity<ProtectionRelay>().ToTable("ProtectionRelays");
            modelBuilder.Entity<ProtectionRelay>().Property(x => x.ProtectionRelayId).HasColumnName("rowid").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<ProtectionRelay>().HasKey(x => x.ProtectionRelayId);
            modelBuilder.Entity<ProtectionRelay>().Property(x => x.AssetNumber).HasColumnName("AssetNumber");
            modelBuilder.Entity<ProtectionRelay>().Property(x => x.Prot).HasColumnName("Prot");
            modelBuilder.Entity<ProtectionRelay>().Property(x => x.RelayModel).HasColumnName("RelayModel");
            modelBuilder.Entity<ProtectionRelay>().Property(x => x.SettingsLocation).HasColumnName("SettingsLocation");
            
            modelBuilder.Entity<ProtectionRelay>().HasMany(x => x.ProtectionElements).WithRequired(x=> x.Relay).HasForeignKey(x => x.ProtectionRelayId);

            modelBuilder.Entity<ProtectionElement>().ToTable("ProtectionElements");
            modelBuilder.Entity<ProtectionElement>().HasKey(x => x.ProtectionElementId);
            modelBuilder.Entity<ProtectionElement>().Property(x => x.ProtectionElementId).HasColumnName("rowid").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<ProtectionElement>().Property(x => x.ANSIName).HasColumnName("ANSIName");
            modelBuilder.Entity<ProtectionElement>().Property(x => x.Curve).HasColumnName("Curve");
            modelBuilder.Entity<ProtectionElement>().Property(x => x.DeadTime1).HasColumnName("DeadTime1");
            modelBuilder.Entity<ProtectionElement>().Property(x => x.DeadTime2).HasColumnName("DeadTime2");
            modelBuilder.Entity<ProtectionElement>().Property(x => x.DeadTime3).HasColumnName("DeadTime3");
            modelBuilder.Entity<ProtectionElement>().Property(x => x.DefT).HasColumnName("DefT");
            modelBuilder.Entity<ProtectionElement>().Property(x => x.Pickup).HasColumnName("Pickup");
            modelBuilder.Entity<ProtectionElement>().Property(x => x.ReclaimTime).HasColumnName("ReclaimTime");
            modelBuilder.Entity<ProtectionElement>().Property(x => x.TMS).HasColumnName("TMS");
            modelBuilder.Entity<ProtectionElement>().Property(x => x.TripsToLockout).HasColumnName("TripsToLockout");

            base.OnModelCreating(modelBuilder);
        }
        

        public DbSet<ProtectedPlant> ProtectedPlant { get; set; }
        public DbSet<ProtectionElement> ProtectionElements { get; set; }
        public DbSet<ProtectionRelay> ProtectionRelays { get; set; }

    }
}
