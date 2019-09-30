using System;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.DataModel.Events;
using Infrastructure.DataModel.MapperEntities;
using Infrastructure.DataModel.Misc;
using Infrastructure.DataModel.People;
using Infrastructure.DataModel.Security;
using Infrastructure.DataModel.Service;
using Infrastructure.DataModel.ServiceAttributes;
using Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;

namespace Infrastructure.DataModel
{
    public abstract class DataContext : DbContext
    {
        #region Constructors

        protected DataContext(IOptions<DatabaseOptions> options)
            : base(options.Value.BuildOptions())
        {
        }

        protected DataContext(DbContextOptions options)
            : base(options)
        {
        }

        #endregion

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                return base.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        #region DbSets

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserClaim> Claims { get; set; }
        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<RealPerson> RealPeople { get; set; }

        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<ServiceSlot> ServiceSlots { get; set; }
        public virtual DbSet<EventServiceModel> EventService { get; set; }
        public virtual DbSet<ServiceType> ServiceTypes { get; set; }
        public virtual DbSet<AttendeeRelationship> AttendeeRelationships { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<ServiceAgreement> ServiceAgreements { get; set; }
        public virtual DbSet<ServiceAttributeSpecification> ServiceAttributeSpecifications { get; set; }
        public virtual DbSet<ServiceAgreementAttributeSpecification> ServiceAgreementAttributeSpecifications { get; set; }
        public virtual DbSet<ServiceAttributeSpecificationBase> ServiceAttributeSpecificationBases { get; set; }
        public virtual DbSet<ServiceAgreementAttribute> ServiceAgreementAttributes { get; set; }
        public virtual DbSet<ServiceAttribute> ServiceAttributes { get; set; }

        #endregion DbSets

        #region Configuration

        private static void ConfigureEventPersonRelationship(EntityTypeBuilder<AttendeeRelationship> builder)
        {
            builder.HasOne(x => x.Person)
                   .WithMany(x => x.EventPersonRelationships)
                   .IsRequired()
                   .HasForeignKey(x => x.PersonId);

            builder.HasOne(x => x.Event)
                   .WithMany(x => x.AttendeeRelationships)
                   .IsRequired()
                   .HasForeignKey(x => x.EventId);

            builder.HasKey(x => new {x.EventId, x.PersonId});
        }

        private static void ConfigurePerson(EntityTypeBuilder<Person> builder)
        {
        }

        private static void ConfigureRealPerson(EntityTypeBuilder<RealPerson> builder)
        {
            builder.HasOne(x => x.User)
                   .WithOne(x => x.Person)
                   .IsRequired()
                   .HasForeignKey<RealPerson>(x => x.UserId);
        }

        private static void ConfigureEvent(EntityTypeBuilder<Event> builder)
        {
            builder.HasOne(x => x.Host)
                   .WithMany(x => x.HostedEvents)
                   .IsRequired()
                   .HasForeignKey(x => x.HostId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Location)
                   .WithMany()
                   .IsRequired()
                   .HasForeignKey(x => x.LocationId)
                   .OnDelete(DeleteBehavior.Cascade);
        }

        private static void ConfigureServiceSlot(EntityTypeBuilder<ServiceSlot> builder)
        {
            builder.HasOne(x => x.Event)
                   .WithMany(x => x.EventServiceSlots)
                   .IsRequired()
                   .HasForeignKey(x => x.EventId);

            builder.HasOne(x => x.Type)
                   .WithMany()
                   .IsRequired()
                   .HasForeignKey(x => x.TypeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd();
            
            builder.HasKey(x => new {x.Id, x.EventId});
        }

        private static void ConfigureEventService(EntityTypeBuilder<EventServiceModel> builder)
        {
            builder.HasOne(x => x.Type)
                   .WithMany()
                   .IsRequired()
                   .HasForeignKey(x => x.TypeId);

            builder.HasOne(x => x.Location)
                   .WithMany()
                   .IsRequired()
                   .HasForeignKey(x => x.LocationId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Person)
                   .WithMany()
                   .IsRequired()
                   .HasForeignKey(x => x.PersonId)
                   .OnDelete(DeleteBehavior.Cascade);
        }

        private static void ConfigureServiceAttributeSpecificationBasess(EntityTypeBuilder<ServiceAttributeSpecificationBase> builder)
        {
            builder.HasOne(x => x.ServiceType)
                   .WithMany()
                   .HasForeignKey(x => x.ServiceTypeId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd();
            
            builder.HasKey(x => new {x.Id, x.ServiceTypeId});
        }

        private static void ConfigureServiceAttribute(EntityTypeBuilder<ServiceAttribute> builder)
        {
            builder.HasOne(x => x.EventServiceModel)
                   .WithMany()
                   .HasForeignKey(x => x.EventServiceModelId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ServiceAttributeSpecification)
                   .WithMany()
                   .HasForeignKey(x => new {x.ServiceAttributeSpecificationId, x.ServiceTypeId})
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasKey(x => new {x.EventServiceModelId, x.ServiceTypeId, x.ServiceAttributeSpecificationId});
        }

        private static void ConfigureServiceAgreementAttribute(EntityTypeBuilder<ServiceAgreementAttribute> builder)
        {
            builder.HasOne(x => x.ServiceAgreement)
                   .WithMany()
                   .HasForeignKey(x => new {x.EventId, x.ServiceSlotId})
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ServiceAttributeSpecification)
                   .WithMany()
                   .HasForeignKey(x => new {x.ServiceAgreementAttributeSpecificationId, x.ServiceTypeId})
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasKey(x => new {x.EventId, x.ServiceAgreementAttributeSpecificationId, x.ServiceSlotId, x.ServiceTypeId});
        }

        private static void ConfigureServiceAgreement(EntityTypeBuilder<ServiceAgreement> builder)
        {
            builder.HasOne(x => x.EventServiceModel)
                   .WithMany()
                   .HasForeignKey(x => x.EventServiceModelId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ServiceSlot)
                   .WithOne()
                   .HasForeignKey<ServiceAgreement>(x => new {x.ServiceSlotId,x.EventId})
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasKey(x => new {x.EventId, x.ServiceSlotId});
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigurePerson(modelBuilder.Entity<Person>());
            ConfigureRealPerson(modelBuilder.Entity<RealPerson>());
            ConfigureEvent(modelBuilder.Entity<Event>());
            ConfigureServiceSlot(modelBuilder.Entity<ServiceSlot>());
            ConfigureEventService(modelBuilder.Entity<EventServiceModel>());
            ConfigureEventPersonRelationship(modelBuilder.Entity<AttendeeRelationship>());
            ConfigureServiceAttributeSpecificationBasess(modelBuilder.Entity<ServiceAttributeSpecificationBase>());
            ConfigureServiceAttribute(modelBuilder.Entity<ServiceAttribute>());
            ConfigureServiceAgreementAttribute(modelBuilder.Entity<ServiceAgreementAttribute>());
            ConfigureServiceAgreement(modelBuilder.Entity<ServiceAgreement>());

            base.OnModelCreating(modelBuilder);
        }

        #endregion
    }
}