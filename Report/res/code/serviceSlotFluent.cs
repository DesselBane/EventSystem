private static void ConfigureServiceSlot(EntityTypeBuilder<ServiceSlot> builder)
        {
            builder.HasOne(x => x.Event)
                   .WithMany(x => x.EventServiceSlots)
                   .IsRequired()
                   .HasForeignKey(x => x.EventId);

            builder.HasOne(x => x.Type) |\label{line:fkFluent}|
                   .WithMany()
                   .IsRequired()
                   .HasForeignKey(x => x.TypeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Id) |\label{line:valueGenFluent}|
                   .ValueGeneratedOnAdd();

            builder.HasKey(x => new {x.Id, x.EventId}); |\label{line:pkFluent}|
        }
