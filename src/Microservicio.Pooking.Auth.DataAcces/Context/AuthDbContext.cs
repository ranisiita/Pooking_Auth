using Microservicio.Pooking.Auth.DataAccess.Configurations;
using Microservicio.Pooking.Auth.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.Pooking.Auth.DataAccess.Context;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options) { }

    public DbSet<UsuarioAppEntity> UsuariosApp => Set<UsuarioAppEntity>();
    public DbSet<RolEntity> Roles => Set<RolEntity>();
    public DbSet<UsuarioRolEntity> UsuariosRoles => Set<UsuarioRolEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UsuarioAppConfiguration());
        modelBuilder.ApplyConfiguration(new RolConfiguration());
        modelBuilder.ApplyConfiguration(new UsuarioRolConfiguration());
    }
}