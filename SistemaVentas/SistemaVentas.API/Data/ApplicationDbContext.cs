using Microsoft.EntityFrameworkCore;
using SistemaVentas.API.Data.Repositories;
using SistemaVentas.Core.Entities;

namespace SistemaVentas.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }
        public DbSet<StockVerificacionResult> StockVerificacionResults { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.IdUsuario);
                entity.Property(e => e.NombreUsuario).IsRequired().HasMaxLength(50);
                entity.Property(e => e.NombreCompleto).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Rol).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Activo).HasDefaultValue(true);
                entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");

                entity.HasIndex(e => e.NombreUsuario).IsUnique();
            });

            // Configuración Producto
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasKey(e => e.IdPro);
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Producto1).IsRequired().HasMaxLength(100)
                    .HasColumnName("Producto");
                entity.Property(e => e.Precio).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Stock).HasDefaultValue(0);
                entity.Property(e => e.Activo).HasDefaultValue(true);
                entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");

                entity.HasIndex(e => e.Codigo).IsUnique();
            });

            // Configuración Venta
            modelBuilder.Entity<Venta>(entity =>
            {
                entity.HasKey(e => e.IdVenta);
                entity.Property(e => e.Fecha).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Vendedor).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SubTotal).HasColumnType("decimal(10,2)");
                entity.Property(e => e.TotalIVA).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Total).HasColumnType("decimal(10,2)");

                entity.HasOne(e => e.Usuario)
                    .WithMany(u => u.Ventas)
                    .HasForeignKey(e => e.IdUsuario)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración DetalleVenta
            modelBuilder.Entity<DetalleVenta>(entity =>
            {
                entity.HasKey(e => e.IdDe);
                entity.Property(e => e.Fecha).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Precio).HasColumnType("decimal(10,2)");
                entity.Property(e => e.IVA).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Total).HasColumnType("decimal(10,2)");

                entity.HasOne(e => e.Venta)
                    .WithMany(v => v.DetalleVentas)
                    .HasForeignKey(e => e.IdVenta)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Producto)
                    .WithMany(p => p.DetalleVentas)
                    .HasForeignKey(e => e.IdPro)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<StockVerificacionResult>().HasNoKey();
        }
    }
}