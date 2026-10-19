using EndpointResolver.Models;
using Microsoft.EntityFrameworkCore;

namespace EndpointResolver.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }

    public DbSet<Teacher> Teachers { get; set; }
}