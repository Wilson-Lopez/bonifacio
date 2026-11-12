using System.ComponentModel.DataAnnotations;

namespace AcademiaApi.Models;

public class Instructor
{
    [Key]
    public int IdInstructor { get; set; }

    [Required, MaxLength(100)]
    public string Nombres { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Apellidos { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Especialidad { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    // Un instructor dicta muchos cursos (1 - N)
    public ICollection<Curso> Cursos { get; set; } = new List<Curso>();
}
