using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ThreeCats.Domain.Entities;
using ThreeCats.Infrastructure.Identity;

namespace ThreeCats.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Client> Clients => Set<Client>(); 
    public DbSet<Doctor> Doctors => Set<Doctor>(); 
    public DbSet<DoctorSchedule> DoctorSchedules => Set<DoctorSchedule>(); 
    public DbSet<Pet> Pets => Set<Pet>(); 
    public DbSet<Service> Services => Set<Service>(); 
    public DbSet<Appointment> Appointments => Set<Appointment>(); 
    public DbSet<Visit> Visits => Set<Visit>(); 
    public DbSet<VisitResult> VisitResults => Set<VisitResult>(); 
    public DbSet<Vaccination> Vaccinations => Set<Vaccination>(); 
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
