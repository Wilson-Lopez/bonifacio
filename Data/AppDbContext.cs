using AcademiaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AcademiaApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Instructor> Instructores => Set<Instructor>();
    public DbSet<Estudiante> Estudiantes => Set<Estudiante>();
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<Matricula> Matriculas => Set<Matricula>();
    public DbSet<Pago> Pagos => Set<Pago>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Nombres de tabla igual que en el diagrama
        modelBuilder.Entity<Instructor>().ToTable("Instructor");
        modelBuilder.Entity<Estudiante>().ToTable("Estudiante");
        modelBuilder.Entity<Curso>().ToTable("Curso");
        modelBuilder.Entity<Matricula>().ToTable("Matricula");
        modelBuilder.Entity<Pago>().ToTable("Pago");

        // El email no se puede repetir
        modelBuilder.Entity<Instructor>().HasIndex(i => i.Email).IsUnique();
        modelBuilder.Entity<Estudiante>().HasIndex(e => e.Email).IsUnique();

        // La fecha de registro la pone la base de datos si no se envía
        modelBuilder.Entity<Estudiante>()
            .Property(e => e.FechaRegistro)
            .HasDefaultValueSql("GETDATE()");

        // Relaciones: no se borra en cascada para no perder información
        modelBuilder.Entity<Curso>()
            .HasOne(c => c.Instructor)
            .WithMany(i => i.Cursos)
            .HasForeignKey(c => c.IdInstructor)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Matricula>()
            .HasOne(m => m.Estudiante)
            .WithMany(e => e.Matriculas)
            .HasForeignKey(m => m.IdEstudiante)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Matricula>()
            .HasOne(m => m.Curso)
            .WithMany(c => c.Matriculas)
            .HasForeignKey(m => m.IdCurso)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Pago>()
            .HasOne(p => p.Matricula)
            .WithMany(m => m.Pagos)
            .HasForeignKey(p => p.IdMatricula)
            .OnDelete(DeleteBehavior.Restrict);

        // Datos iniciales (se insertan con la migración "DatosIniciales")
        modelBuilder.Entity<Instructor>().HasData(
            new Instructor { IdInstructor = 1, Nombres = "Carlos", Apellidos = "Ramírez Torres", Especialidad = "Desarrollo Web", Email = "carlos.ramirez@academia.com", Activo = true },
            new Instructor { IdInstructor = 2, Nombres = "Ana", Apellidos = "Gómez Salazar", Especialidad = "Base de Datos", Email = "ana.gomez@academia.com", Activo = true },
            new Instructor { IdInstructor = 3, Nombres = "Luis", Apellidos = "Fernández Castro", Especialidad = "Redes y Seguridad", Email = "luis.fernandez@academia.com", Activo = false }
        );

        modelBuilder.Entity<Estudiante>().HasData(
            new Estudiante { IdEstudiante = 1, Nombres = "María", Apellidos = "López Quispe", Email = "maria.lopez@alumnos.academia.com", Telefono = "987654321", FechaRegistro = new DateTime(2026, 3, 2, 9, 15, 0), Activo = true },
            new Estudiante { IdEstudiante = 2, Nombres = "José", Apellidos = "Pérez Huamán", Email = "jose.perez@alumnos.academia.com", Telefono = "912345678", FechaRegistro = new DateTime(2026, 3, 5, 14, 30, 0), Activo = true },
            new Estudiante { IdEstudiante = 3, Nombres = "Lucía", Apellidos = "Torres Mendoza", Email = "lucia.torres@alumnos.academia.com", Telefono = null, FechaRegistro = new DateTime(2026, 4, 10, 10, 0, 0), Activo = true }
        );
    }
}
