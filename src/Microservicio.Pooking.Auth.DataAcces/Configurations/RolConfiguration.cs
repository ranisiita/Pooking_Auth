using Microservicio.Pooking.Auth.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservicio.Pooking.Auth.DataAccess.Configurations;

/// <summary>
/// Configuración de EF Core para la tabla booking.rol.
/// Motor: PostgreSQL (Npgsql).
/// </summary>
public class RolConfiguration : IEntityTypeConfiguration<RolEntity>
{
    public void Configure(EntityTypeBuilder<RolEntity> builder)
    {
        // -------------------------------------------------------------------------
        // Tabla y esquema
        // -------------------------------------------------------------------------
        builder.ToTable("rol", "booking");

        // -------------------------------------------------------------------------
        // [1] Identificación técnica
        // -------------------------------------------------------------------------
        builder.HasKey(r => r.IdRol);

        builder.Property(r => r.IdRol)
               .HasColumnName("id_rol")
               .UseIdentityColumn();              // mapea SERIAL de PostgreSQL

        builder.Property(r => r.RolGuid)
               .HasColumnName("rol_guid")
               .HasDefaultValueSql("gen_random_uuid()")   // función nativa de PostgreSQL
               .IsRequired();

        builder.HasIndex(r => r.RolGuid)
               .IsUnique()
               .HasDatabaseName("uq_rol_guid");

        // -------------------------------------------------------------------------
        // [2] Datos funcionales
        // -------------------------------------------------------------------------
        builder.Property(r => r.NombreRol)
               .HasColumnName("nombre_rol")
               .HasMaxLength(50)
               .IsRequired();

        builder.HasIndex(r => r.NombreRol)
               .IsUnique()
               .HasDatabaseName("uq_rol_nombre");

        builder.Property(r => r.DescripcionRol)
               .HasColumnName("descripcion_rol")
               .HasMaxLength(200)
               .IsRequired(false);

        // -------------------------------------------------------------------------
        // [3] Estado y ciclo de vida
        // -------------------------------------------------------------------------
        builder.Property(r => r.EstadoRol)
               .HasColumnName("estado_rol")
               .HasColumnType("CHAR(3)")
               .HasDefaultValue("ACT")
               .IsRequired();

        builder.ToTable(t => t.HasCheckConstraint(
            "chk_rol_estado",
            "estado_rol IN ('ACT', 'INA')"
        ));

        builder.Property(r => r.EsEliminado)
               .HasColumnName("es_eliminado")
               .HasDefaultValue(false)
               .IsRequired();

        builder.Property(r => r.Activo)
               .HasColumnName("activo")
               .HasDefaultValue(true)
               .IsRequired();

        // -------------------------------------------------------------------------
        // [4] Auditoría
        // -------------------------------------------------------------------------
        builder.Property(r => r.FechaRegistroUtc)
               .HasColumnName("fecha_registro_utc")
               .HasColumnType("TIMESTAMP(0)")                          // tipo PostgreSQL
               .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'")        // función PostgreSQL
               .IsRequired();

        builder.Property(r => r.CreadoPorUsuario)
               .HasColumnName("creado_por_usuario")
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(r => r.ModificadoPorUsuario)
               .HasColumnName("modificado_por_usuario")
               .HasMaxLength(100)
               .IsRequired(false);

        builder.Property(r => r.FechaModificacionUtc)
               .HasColumnName("fecha_modificacion_utc")
               .HasColumnType("TIMESTAMP(0)")
               .IsRequired(false);

        // -------------------------------------------------------------------------
        // Navegación
        // -------------------------------------------------------------------------
        builder.HasMany(r => r.UsuariosRoles)
               .WithOne(ur => ur.Rol)
               .HasForeignKey(ur => ur.IdRol)
               .OnDelete(DeleteBehavior.NoAction);
    }
}