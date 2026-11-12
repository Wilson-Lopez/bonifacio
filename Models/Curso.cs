using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademiaApi.Models;

public class Curso
{
    [Key]
    public int IdCurso { get; set; }

    [Required, MaxLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Descripcion { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Precio { get; set; }

    public int DuracionHoras { get; set; }

    // FK -> Instructor
    public int IdInstructor { get; set; }

    [ForeignKey(nameof(IdInstructor))]
    public Instructor? Instructor { get; set; }

    public bool Activo { get; set; } = true;

    // Un curso tiene muchas matrículas (1 - N)
    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
}
