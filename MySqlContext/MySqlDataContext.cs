using Infrastructure.DataModel;
using Infrastructure.DataModel.Events;
using Infrastructure.DataModel.ServiceAttributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MySqlContext
{
    public sealed class MySqlDataContext : DataContext
    {
        #region Constructors

        public MySqlDataContext(DbContextOptions options)
            : base(options)
        {
        }

        #endregion
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            ConfigureServiceSlotIdentity(modelBuilder.Entity<ServiceSlot>());
            ConfigureServiceSpecificationBaseIdentity(modelBuilder.Entity<ServiceAttributeSpecificationBase>());
        }

        private void ConfigureServiceSlotIdentity(EntityTypeBuilder<ServiceSlot> entity)
        {
            entity.Property(x => x.Id)
                  .UseMySqlIdentityColumn();
        }

        private void ConfigureServiceSpecificationBaseIdentity(EntityTypeBuilder<ServiceAttributeSpecificationBase> entity)
        {
            entity.Property(x => x.Id)
                  .UseMySqlIdentityColumn();
        }
        
    }
}