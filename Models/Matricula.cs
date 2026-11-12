using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademiaApi.Models;

public class Matricula
{
    [Key]
    public int IdMatricula { get; set; }

    // FK -> Estudiante
    public int IdEstudiante { get; set; }

    [ForeignKey(nameof(IdEstudiante))]
    public Estudiante? Estudiante { get; set; }

    // FK -> Curso
    public int IdCurso { get; set; }

    [ForeignKey(nameof(IdCurso))]
    public Curso? Curso { get; set; }

    public DateTime FechaMatricula { get; set; }

    [Required, MaxLength(30)]
    public string Estado { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    public decimal MontoTotal { get; set; }

    // Una matrícula tiene muchos pagos (1 - N)
    public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
