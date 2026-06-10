using Microsoft.EntityFrameworkCore;
using test_A.Entities;

namespace test_A.Infrastructure;

public class ClinicDbContext : DbContext
{
    public ClinicDbContext(DbContextOptions<ClinicDbContext> options) : base(options)
    {
    }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<MedicalService> MedicalServices => Set<MedicalService>();
    public DbSet<AppointmentService> AppointmentServices => Set<AppointmentService>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(eb =>
        {
            eb.ToTable("Patients");
            eb.HasKey(p => p.PatientId);
            eb.Property(p => p.FirstName).IsRequired().HasMaxLength(50);
            eb.Property(p => p.LastName).IsRequired().HasMaxLength(100);
            eb.Property(p => p.Phone).IsRequired().HasMaxLength(9);
            eb.Property(p => p.DateOfBirth).IsRequired();

            eb.HasMany(p => p.Appointments)
                .WithOne(a => a.Patient)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Doctor>(eb =>
        {
            eb.ToTable("Doctors");
            eb.HasKey(d => d.DoctorId);
            eb.Property(d => d.FirstName).IsRequired().HasMaxLength(50);
            eb.Property(d => d.LastName).IsRequired().HasMaxLength(100);
            eb.Property(d => d.Specialization).IsRequired().HasMaxLength(100);
            eb.Property(d => d.Phone).IsRequired().HasMaxLength(9);

            eb.HasMany(d => d.Appointments)
                .WithOne(a => a.Doctor)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Appointment>(eb =>
        {
            eb.ToTable("Appointments");
            eb.HasKey(a => a.AppointmentId);
            eb.Property(a => a.AppointmentDate).HasColumnType("date").IsRequired();
            eb.Property(a => a.Status).IsRequired().HasMaxLength(50);

            eb.HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            eb.HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            eb.HasMany(a => a.AppointmentServices)
                .WithOne(x => x.Appointment)
                .HasForeignKey(x => x.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MedicalService>(eb =>
        {
            eb.ToTable("Medical_Services");
            eb.HasKey(ms => ms.ServiceId);
            eb.Property(ms => ms.Name).IsRequired().HasMaxLength(100);
            eb.Property(ms => ms.Description).IsRequired().HasMaxLength(100);
            eb.Property(ms => ms.Price).HasColumnType("decimal(10,2)").IsRequired();
            eb.Property(ms => ms.DurationMinutes).IsRequired();

            eb.HasMany(ms => ms.AppointmentServices)
                .WithOne(x => x.MedicalService)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AppointmentService>(eb =>
        {
            eb.ToTable("Appointment_Services");
            eb.HasKey(x => new { x.AppointmentId, x.ServiceId });
            eb.Property(x => x.Quantity).IsRequired();
            eb.Property(x => x.PerformedAt).HasColumnType("date").IsRequired();

            eb.HasOne(x => x.Appointment)
                .WithMany(a => a.AppointmentServices)
                .HasForeignKey(x => x.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            eb.HasOne(x => x.MedicalService)
                .WithMany(ms => ms.AppointmentServices)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
