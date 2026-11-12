using System.ComponentModel.DataAnnotations;

namespace AcademiaApi.Models;

public class Estudiante
{
    [Key]
    public int IdEstudiante { get; set; }

    [Required, MaxLength(100)]
    public string Nombres { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Apellidos { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Telefono { get; set; }

    public DateTime FechaRegistro { get; set; }

    public bool Activo { get; set; } = true;

    // Un estudiante tiene muchas matrículas (1 - N)
    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
}
