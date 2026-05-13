using Microservicio.Pooking.Auth.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservicio.Pooking.Auth.DataAccess.Configurations;

/// <summary>
/// Configuración de EF Core para la tabla puente booking.usuarios_roles.
/// Motor: PostgreSQL (Npgsql).
/// </summary>
public class UsuarioRolConfiguration : IEntityTypeConfiguration<UsuarioRolEntity>
{
    public void Configure(EntityTypeBuilder<UsuarioRolEntity> builder)
    {
        // -------------------------------------------------------------------------
        // Tabla y esquema
        // -------------------------------------------------------------------------
        builder.ToTable("usuarios_roles", "booking");

        // -------------------------------------------------------------------------
        // [1] Identificación técnica
        // -------------------------------------------------------------------------
        builder.HasKey(ur => ur.IdUsuarioRol);

        builder.Property(ur => ur.IdUsuarioRol)
               .HasColumnName("id_usuario_rol")
               .UseIdentityColumn();              // mapea SERIAL de PostgreSQL

        // -------------------------------------------------------------------------
        // [2] Claves foráneas
        // -------------------------------------------------------------------------
        builder.Property(ur => ur.IdUsuario)
               .HasColumnName("id_usuario")
               .IsRequired();

        builder.Property(ur => ur.IdRol)
               .HasColumnName("id_rol")
               .IsRequired();

        // Índice único compuesto: un usuario no puede tener el mismo rol dos veces
        builder.HasIndex(ur => new { ur.IdUsuario, ur.IdRol })
               .IsUnique()
               .HasDatabaseName("uq_usuarios_roles_usuario_rol");

        // Índices de búsqueda individuales
        builder.HasIndex(ur => ur.IdUsuario)
               .HasDatabaseName("idx_usuarios_roles_usuario");

        builder.HasIndex(ur => ur.IdRol)
               .HasDatabaseName("idx_usuarios_roles_rol");

        // -------------------------------------------------------------------------
        // [3] Estado y ciclo de vida
        // -------------------------------------------------------------------------
        builder.Property(ur => ur.EstadoUsuarioRol)
               .HasColumnName("estado_usuario_rol")
               .HasColumnType("CHAR(3)")
               .HasDefaultValue("ACT")
               .IsRequired();

        builder.ToTable(t => t.HasCheckConstraint(
            "chk_usuarios_roles_estado",
            "estado_usuario_rol IN ('ACT', 'INA')"
        ));

        builder.Property(ur => ur.EsEliminado)
               .HasColumnName("es_eliminado")
               .HasDefaultValue(false)
               .IsRequired();

        builder.Property(ur => ur.Activo)
               .HasColumnName("activo")
               .HasDefaultValue(true)
               .IsRequired();

        // -------------------------------------------------------------------------
        // [4] Auditoría
        // -------------------------------------------------------------------------
        builder.Property(ur => ur.FechaRegistroUtc)
               .HasColumnName("fecha_registro_utc")
               .HasColumnType("TIMESTAMP(0)")                          // tipo PostgreSQL
               .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'")        // función PostgreSQL
               .IsRequired();

        builder.Property(ur => ur.CreadoPorUsuario)
               .HasColumnName("creado_por_usuario")
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(ur => ur.ModificadoPorUsuario)
               .HasColumnName("modificado_por_usuario")
               .HasMaxLength(100)
               .IsRequired(false);

        builder.Property(ur => ur.FechaModificacionUtc)
               .HasColumnName("fecha_modificacion_utc")
               .HasColumnType("TIMESTAMP(0)")
               .IsRequired(false);

        // -------------------------------------------------------------------------
        // Navegación
        // -------------------------------------------------------------------------
        builder.HasOne(ur => ur.Usuario)
               .WithMany(u => u.UsuariosRoles)
               .HasForeignKey(ur => ur.IdUsuario)
               .OnDelete(DeleteBehavior.NoAction)
               .HasConstraintName("fk_usuarios_roles_usuario");

        builder.HasOne(ur => ur.Rol)
               .WithMany(r => r.UsuariosRoles)
               .HasForeignKey(ur => ur.IdRol)
               .OnDelete(DeleteBehavior.NoAction)
               .HasConstraintName("fk_usuarios_roles_rol");
    }
}